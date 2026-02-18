using Application.Dtos.Addresses.Request;
using Application.Dtos.Persons;
using Application.Dtos.Persons.Request;
using Application.Ports;
using Application.Services;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Pan.Backend.Tests.Application.Services
{
    public sealed class PersonServiceUpdateTests
    {
        private readonly Mock<IIndividualRepository> _individualRepo = new(MockBehavior.Strict);
        private readonly Mock<ICompanyRepository> _companyRepo = new(MockBehavior.Strict);
        private readonly Mock<IAddressRepository> _addressRepo = new(MockBehavior.Strict);
        private readonly Mock<IAddressService> _addressService = new(MockBehavior.Strict);

        private PersonService CreateSut()
            => new(_individualRepo.Object, _companyRepo.Object, _addressRepo.Object, _addressService.Object);

        [Fact]
        public async Task UpdateAsync_ShouldUpdateIndividualNameAndCpf_WhenProvided()
        {
            // Arrange
            var sut = CreateSut();

            var existing = new Individual(                 
                name: "Maria",
                cpf: Cpf.From("529.982.247-25"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: "Apto 1"
            );

            var id = existing.Id;

            _individualRepo
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            var request = new UpdatePersonRequest
            {
                Name = "  Maria Silva  ",
                Cpf = "390.533.447-05"
            };

            _individualRepo
                .Setup(x => x.UpdateAsync(It.Is<Individual>(i =>
                    i.Name == "Maria Silva" &&
                    i.Cpf.Value == "39053344705"
                ), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _individualRepo
                .Setup(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Company não deve ser consultada
            _companyRepo
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company?)null);

            // Act
            var result = await sut.UpdateAsync(id, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(PersonType.Individual, result!.Type);
            Assert.Equal("Maria Silva", result.Name);
            Assert.Equal("39053344705", result.Cpf);

            _addressRepo.VerifyNoOtherCalls();
            _addressService.VerifyNoOtherCalls();
            _companyRepo.VerifyNoOtherCalls();
            _individualRepo.VerifyAll();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateIndividualAddressId_WhenAddressProvidedAndResolved()
        {
            // Arrange
            var sut = CreateSut();
            var id = Guid.NewGuid();

            var existing = new Individual(
                name: "Maria",
                cpf: Cpf.From("529.982.247-25"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: null
            );

            _individualRepo
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // ResolveOrCreateAddress: tenta buscar por CEP no DB
            var resolvedAddressId = Guid.NewGuid();

            _addressRepo
                .Setup(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Address(
                    cep: Cep.From("60181-225"),
                    street: "Rua X",
                    neighborhood: "Bairro",
                    city: "Fortaleza",
                    state: "CE"                    
                ));

            // - capturar o Address retornado pelo repo e verificar que o Person recebeu aquele Id.
            Address addr = null!;
            _addressRepo
                .Setup(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    addr = new Address(
                        Cep.From("60181-225"),
                        "Rua X",
                        "Bairro",
                        "Fortaleza",
                        "CE"
                    );
                    return addr;
                });

            var request = new UpdatePersonRequest
            {
                Address = new CreateAddressRequest { Cep = "60181-225" }
            };

            _individualRepo
                .Setup(x => x.UpdateAsync(It.Is<Individual>(i =>
                    i.AddressId == addr.Id
                ), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _individualRepo
                .Setup(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            var result = await sut.UpdateAsync(id, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(PersonType.Individual, result!.Type);
            Assert.Equal(addr.Id, result.AddressId);

            _addressService.VerifyNoOtherCalls(); // não criou address, só resolveu por CEP
            _addressRepo.VerifyAll();
            _companyRepo.VerifyNoOtherCalls();
            _individualRepo.VerifyAll();
        }

        [Fact]
        public async Task UpdateAsync_ShouldClearIndividualComplement_WhenEmptyStringProvided()
        {
            // Arrange
            var sut = CreateSut();
            var id = Guid.NewGuid();

            var existing = new Individual(
                name: "Maria",
                cpf: Cpf.From("529.982.247-25"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: "Apto 1"
            );

            _individualRepo
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            var request = new UpdatePersonRequest
            {
                AddressComplement = ""
            };

            _individualRepo
                .Setup(x => x.UpdateAsync(It.Is<Individual>(i =>
                    i.AddressNumber == "10" &&
                    i.AddressComplement == null
                ), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _individualRepo
                .Setup(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            var result = await sut.UpdateAsync(id, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(PersonType.Individual, result!.Type);

            _addressRepo.VerifyNoOtherCalls();
            _addressService.VerifyNoOtherCalls();
            _companyRepo.VerifyNoOtherCalls();
            _individualRepo.VerifyAll();
        }
    }
}
