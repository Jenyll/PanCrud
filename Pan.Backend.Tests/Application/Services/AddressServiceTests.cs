using Application.Dtos.Addresses.Request;
using Application.Dtos.Addresses.Response;
using Application.Ports;
using Application.Services;
using Application.UseCases.Addresses.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Pan.Backend.Tests.Application.Services
{
    public sealed class AddressServiceTests
    {
        private readonly Mock<ICreateAddress> _create = new(MockBehavior.Strict);
        private readonly Mock<IGetAddress> _get = new(MockBehavior.Strict);
        private readonly Mock<IUpdateAddress> _update = new(MockBehavior.Strict);
        private readonly Mock<IDeleteAddress> _delete = new(MockBehavior.Strict);
        private readonly Mock<IAddressRepository> _repo = new(MockBehavior.Strict);

        private AddressService CreateSut()
            => new(_create.Object, _get.Object, _update.Object, _delete.Object, _repo.Object);

        [Fact]
        public async Task CreateAsync_ShouldDelegateToCreateUseCase()
        {
            var sut = CreateSut();

            var request = new CreateAddressRequest { Cep = "60181-225" };
            var expected = new AddressResponse
            {
                Id = Guid.NewGuid(),
                Cep = "60181225",
                Street = "Rua X",
                Neighborhood = "Bairro",
                City = "Fortaleza",
                State = "CE"
            };

            _create
                .Setup(x => x.ExecuteAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await sut.CreateAsync(request, CancellationToken.None);

            Assert.Equal(expected.Id, result.Id);
            _create.VerifyAll();
            _get.VerifyNoOtherCalls();
            _update.VerifyNoOtherCalls();
            _delete.VerifyNoOtherCalls();
            _repo.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldDelegateToGetUseCase()
        {
            var sut = CreateSut();

            var id = Guid.NewGuid();
            _get
                .Setup(x => x.ExecuteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AddressResponse?)null);

            var result = await sut.GetByIdAsync(id, CancellationToken.None);

            Assert.Null(result);
            _get.VerifyAll();
            _create.VerifyNoOtherCalls();
            _update.VerifyNoOtherCalls();
            _delete.VerifyNoOtherCalls();
            _repo.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateAsync_ShouldDelegateToUpdateUseCase()
        {
            var sut = CreateSut();

            var id = Guid.NewGuid();
            var request = new UpdateAddressRequest { Cep = "58348-000" };

            _update
                .Setup(x => x.ExecuteAsync(id, request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AddressResponse?)null);

            var result = await sut.UpdateAsync(id, request, CancellationToken.None);

            Assert.Null(result);
            _update.VerifyAll();
            _create.VerifyNoOtherCalls();
            _get.VerifyNoOtherCalls();
            _delete.VerifyNoOtherCalls();
            _repo.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDelegateToDeleteUseCase()
        {
            var sut = CreateSut();

            var id = Guid.NewGuid();

            _delete
                .Setup(x => x.ExecuteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await sut.DeleteAsync(id, CancellationToken.None);

            Assert.True(result);
            _delete.VerifyAll();
            _create.VerifyNoOtherCalls();
            _get.VerifyNoOtherCalls();
            _update.VerifyNoOtherCalls();
            _repo.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task LookupByCepAsync_ShouldReturnNull_WhenNotFoundInRepo()
        {
            var sut = CreateSut();

            _repo
                .Setup(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Address?)null);

            var result = await sut.LookupByCepAsync("60181-225", CancellationToken.None);

            Assert.Null(result);

            _repo.VerifyAll();
            _create.VerifyNoOtherCalls();
            _get.VerifyNoOtherCalls();
            _update.VerifyNoOtherCalls();
            _delete.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task LookupByCepAsync_ShouldReturnLookupResponse_WhenFoundInRepo()
        {
            var sut = CreateSut();

            var entity = new Address(
                cep: Cep.From("60181-225"),
                street: "Rua Pascoal de Castro Alves",
                neighborhood: "Vicente Pinzon",
                city: "Fortaleza",
                state: "CE"
            );

            _repo
                .Setup(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await sut.LookupByCepAsync("60181-225", CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("60181225", result!.Cep);
            Assert.Equal("Rua Pascoal de Castro Alves", result.Street);
            Assert.Equal("Vicente Pinzon", result.Neighborhood);
            Assert.Equal("Fortaleza", result.City);
            Assert.Equal("CE", result.State);

            _repo.VerifyAll();
            _create.VerifyNoOtherCalls();
            _get.VerifyNoOtherCalls();
            _update.VerifyNoOtherCalls();
            _delete.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task LookupByCepAsync_ShouldThrow_WhenCepIsInvalid()
        {
            var sut = CreateSut();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                sut.LookupByCepAsync("00000-000", CancellationToken.None));

            _repo.VerifyNoOtherCalls();
            _create.VerifyNoOtherCalls();
            _get.VerifyNoOtherCalls();
            _update.VerifyNoOtherCalls();
            _delete.VerifyNoOtherCalls();
        }
    }
}
