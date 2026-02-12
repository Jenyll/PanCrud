using Application.Dtos.Addresses.Request;
using Application.Dtos.Addresses.Response;
using Application.Dtos.Persons;
using Application.Dtos.Persons.Request;
using Application.Ports;
using Application.Services;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Pan.Backend.Tests.Application.Services
{
    public sealed class PersonServiceCreateCompanyTests
    {
        private readonly Mock<IIndividualRepository> _individualRepo = new(MockBehavior.Strict);
        private readonly Mock<ICompanyRepository> _companyRepo = new(MockBehavior.Strict);
        private readonly Mock<IAddressRepository> _addressRepo = new(MockBehavior.Strict);
        private readonly Mock<IAddressService> _addressService = new(MockBehavior.Strict);

        private PersonService CreateSut()
            => new(_individualRepo.Object, _companyRepo.Object, _addressRepo.Object, _addressService.Object);

        [Fact]
        public async Task CreateAsync_ShouldCreateCompany_WhenAddressIdProvided()
        {
            var sut = CreateSut();
            var addressId = Guid.NewGuid();

            var request = new CreatePersonRequest
            {
                Type = PersonType.Company,
                LegalName = "ACME LTDA",
                Cnpj = "04.252.011/0001-10",
                AddressId = addressId,
                AddressNumber = "500",
                AddressComplement = "Sala 301"
            };

            _companyRepo
                .Setup(x => x.CreateAsync(It.Is<Company>(c =>
                    c.LegalName == "ACME LTDA" &&
                    c.Cnpj.Value == "04252011000110" &&
                    c.AddressId == addressId &&
                    c.AddressNumber == "500" &&
                    c.AddressComplement == "Sala 301"
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company c, CancellationToken _) => c);

            _companyRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company?)null);

            var result = await sut.CreateAsync(request, CancellationToken.None);

            Assert.Equal(PersonType.Company, result.Type);
            Assert.Equal("ACME LTDA", result.LegalName);
            Assert.Equal("04252011000110", result.Cnpj);
            Assert.Equal(addressId, result.AddressId);

            _addressRepo.VerifyNoOtherCalls();
            _addressService.VerifyNoOtherCalls();
            _individualRepo.VerifyNoOtherCalls();
            _companyRepo.VerifyAll();
        }

        [Fact]
        public async Task CreateAsync_ShouldUseExistingAddressFromDb_WhenAddressProvidedAndFoundByCep()
        {
            var sut = CreateSut();

            var request = new CreatePersonRequest
            {
                Type = PersonType.Company,
                LegalName = "ACME LTDA",
                Cnpj = "04.252.011/0001-10",
                Address = new CreateAddressRequest { Cep = "60181-225" },
                AddressNumber = "500",
                AddressComplement = null
            };

            Address existingAddress = null!;
            _addressRepo
                .Setup(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    existingAddress = new Address(
                        Cep.From("60181-225"),
                        street: "Rua X",
                        neighborhood: "Bairro",
                        city: "Fortaleza",
                        state: "CE"
                    );
                    return existingAddress;
                });

            _companyRepo
                .Setup(x => x.CreateAsync(It.Is<Company>(c =>
                    c.AddressId == existingAddress.Id &&
                    c.AddressNumber == "500" &&
                    c.AddressComplement == null
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company c, CancellationToken _) => c);

            _companyRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company?)null);

            var result = await sut.CreateAsync(request, CancellationToken.None);

            Assert.Equal(PersonType.Company, result.Type);
            Assert.Equal(existingAddress.Id, result.AddressId);

            _addressService.VerifyNoOtherCalls();
            _individualRepo.VerifyNoOtherCalls();
            _addressRepo.VerifyAll();
            _companyRepo.VerifyAll();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateAddressViaAddressService_WhenAddressNotFoundInDb()
        {
            var sut = CreateSut();
            var createdAddressId = Guid.NewGuid();

            var request = new CreatePersonRequest
            {
                Type = PersonType.Company,
                LegalName = "ACME LTDA",
                Cnpj = "04.252.011/0001-10",
                Address = new CreateAddressRequest { Cep = "60181-225" },
                AddressNumber = "500",
                AddressComplement = null
            };

            _addressRepo
                .Setup(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Address?)null);

            _addressService
                .Setup(x => x.CreateAsync(It.Is<CreateAddressRequest>(a => a.Cep == "60181-225"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AddressResponse
                {
                    Id = createdAddressId,
                    Cep = "60181225",
                    Street = "Rua Pascoal de Castro Alves",
                    Neighborhood = "Vicente Pinzon",
                    City = "Fortaleza",
                    State = "CE"
                });

            _companyRepo
                .Setup(x => x.CreateAsync(It.Is<Company>(c =>
                    c.AddressId == createdAddressId &&
                    c.AddressNumber == "500"
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company c, CancellationToken _) => c);

            _companyRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company?)null);

            var result = await sut.CreateAsync(request, CancellationToken.None);

            Assert.Equal(PersonType.Company, result.Type);
            Assert.Equal(createdAddressId, result.AddressId);

            _addressRepo.VerifyAll();
            _addressService.VerifyAll();
            _companyRepo.VerifyAll();
            _individualRepo.VerifyNoOtherCalls();
        }
    }
}
