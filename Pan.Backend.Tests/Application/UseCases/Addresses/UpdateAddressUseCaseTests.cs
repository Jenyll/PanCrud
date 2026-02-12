using Application.Dtos.Addresses.Request;
using Application.Dtos.ViaCep;
using Application.Ports;
using Application.UseCases.Addresses.Implement;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pan.Backend.Tests.Application.UseCases.Addresses
{
    public sealed class UpdateAddressUseCaseTests
    {
        private readonly Mock<IAddressRepository> _repo = new(MockBehavior.Strict);
        private readonly Mock<IViaCepClient> _viaCep = new(MockBehavior.Strict);

        [Fact]
        public async Task ExecuteAsync_ShouldReturnNull_WhenAddressNotFound()
        {
            var id = Guid.NewGuid();

            _repo
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Address?)null);

            var sut = new UpdateAddress(_repo.Object, _viaCep.Object);

            var result = await sut.ExecuteAsync(id, new UpdateAddressRequest { Cep = "58348-000" }, CancellationToken.None);

            Assert.Null(result);

            _repo.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _repo.VerifyNoOtherCalls();
            _viaCep.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldUpdateViaCepFields_WhenCepProvidedAndDifferent()
        {
            var address = new Address(
                cep: Cep.From("60181-225"),
                street: "Rua Antiga",
                neighborhood: "Bairro",
                city: "Fortaleza",
                state: "CE"
            );

            _repo
                .Setup(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(address);

            _viaCep
                .Setup(x => x.GetAsync("58348000", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ViaCepAddress
                {
                    Cep = "58348-000",
                    Street = "Rua Nova",
                    Neighborhood = "Centro",
                    City = "Riachão do Poço",
                    State = "PB",
                    Error = false
                });

            _repo
                .Setup(x => x.UpdateAsync(It.Is<Address>(a =>
                    a.Id == address.Id &&
                    a.Cep.Value == "58348000" &&
                    a.Street == "Rua Nova" &&
                    a.Neighborhood == "Centro" &&
                    a.City == "Riachão do Poço" &&
                    a.State == "PB"
                ), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var sut = new UpdateAddress(_repo.Object, _viaCep.Object);

            var request = new UpdateAddressRequest { Cep = "58348-000" };

            var result = await sut.ExecuteAsync(address.Id, request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("58348000", result!.Cep.Replace("-", ""));
            Assert.Equal("Rua Nova", result.Street);
            Assert.Equal("Centro", result.Neighborhood);
            Assert.Equal("Riachão do Poço", result.City);
            Assert.Equal("PB", result.State);

            _repo.Verify(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()), Times.Once);
            _viaCep.Verify(x => x.GetAsync("58348000", It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(x => x.UpdateAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Once);
            _repo.VerifyNoOtherCalls();
            _viaCep.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRefreshViaCepFields_WhenCepProvidedAndSame()
        {
            var address = new Address(
                cep: Cep.From("60181-225"),
                street: "Rua Antiga",
                neighborhood: "Bairro Antigo",
                city: "Fortaleza",
                state: "CE"
            );

            _repo
                .Setup(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(address);

            _viaCep
                .Setup(x => x.GetAsync("60181225", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ViaCepAddress
                {
                    Cep = "60181-225",
                    Street = "Rua Atualizada",
                    Neighborhood = "Vicente Pinzon",
                    City = "Fortaleza",
                    State = "CE",
                    Error = false
                });

            _repo
                .Setup(x => x.UpdateAsync(It.Is<Address>(a =>
                    a.Id == address.Id &&
                    a.Cep.Value == "60181225" &&
                    a.Street == "Rua Atualizada" &&
                    a.Neighborhood == "Vicente Pinzon" &&
                    a.City == "Fortaleza" &&
                    a.State == "CE"
                ), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var sut = new UpdateAddress(_repo.Object, _viaCep.Object);

            var request = new UpdateAddressRequest { Cep = "60181-225" };

            var result = await sut.ExecuteAsync(address.Id, request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("60181225", result!.Cep.Replace("-", ""));
            Assert.Equal("Rua Atualizada", result.Street);
            Assert.Equal("Vicente Pinzon", result.Neighborhood);

            _repo.Verify(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()), Times.Once);
            _viaCep.Verify(x => x.GetAsync("60181225", It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(x => x.UpdateAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Once);
            _repo.VerifyNoOtherCalls();
            _viaCep.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenViaCepReturnsError()
        {
            var address = new Address(
                cep: Cep.From("60181-225"),
                street: "Rua Antiga",
                neighborhood: "Bairro",
                city: "Fortaleza",
                state: "CE"
            );

            _repo
                .Setup(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(address);

            _viaCep
                .Setup(x => x.GetAsync("58348000", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ViaCepAddress { Error = true });

            var sut = new UpdateAddress(_repo.Object, _viaCep.Object);

            var request = new UpdateAddressRequest { Cep = "58348-000" };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                sut.ExecuteAsync(address.Id, request, CancellationToken.None));

            _repo.Verify(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()), Times.Once);
            _viaCep.Verify(x => x.GetAsync("58348000", It.IsAny<CancellationToken>()), Times.Once);

            // não salva se ViaCEP falhar
            _repo.VerifyNoOtherCalls();
            _viaCep.VerifyNoOtherCalls();
        }
    }
}
