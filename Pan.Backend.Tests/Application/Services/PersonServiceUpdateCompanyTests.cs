using Application.Dtos.Persons;
using Application.Dtos.Persons.Request;
using Application.Ports;
using Application.Services;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Pan.Backend.Tests.Application.Services
{
    public sealed class PersonServiceUpdateCompanyTests
    {
        private readonly Mock<IIndividualRepository> _individualRepo = new(MockBehavior.Strict);
        private readonly Mock<ICompanyRepository> _companyRepo = new(MockBehavior.Strict);
        private readonly Mock<IAddressRepository> _addressRepo = new(MockBehavior.Strict);
        private readonly Mock<IAddressService> _addressService = new(MockBehavior.Strict);

        private PersonService CreateSut()
            => new(_individualRepo.Object, _companyRepo.Object, _addressRepo.Object, _addressService.Object);

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCompanyLegalNameAndCnpj_WhenProvided()
        {
            // Arrange
            var sut = CreateSut();

            var existing = new Company(
                legalName: "ACME",
                cnpj: Cnpj.From("04.252.011/0001-10"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: "Sala 1"
            );
            var id = existing.Id;

            _individualRepo
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Individual?)null);

            _companyRepo
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            var request = new UpdatePersonRequest
            {
                LegalName = "  ACME LTDA  ",
                Cnpj = "40.688.134/0001-61"
            };

            _companyRepo
                .Setup(x => x.UpdateAsync(It.Is<Company>(c =>
                    c.LegalName == "ACME LTDA" &&
                    c.Cnpj.Value == "40688134000161"
                ), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _companyRepo
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            var result = await sut.UpdateAsync(id, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(PersonType.Company, result!.Type);
            Assert.Equal("ACME LTDA", result.LegalName);
            Assert.Equal("40688134000161", result.Cnpj);

            _individualRepo.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _individualRepo.VerifyNoOtherCalls();

            _companyRepo.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Exactly(2));
            _companyRepo.Verify(x => x.UpdateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
            _companyRepo.VerifyNoOtherCalls();

            _addressRepo.VerifyNoOtherCalls();
            _addressService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateAsync_ShouldClearCompanyComplement_WhenEmptyStringProvided()
        {
            // Arrange
            var sut = CreateSut();

            var existing = new Company(
                legalName: "ACME",
                cnpj: Cnpj.From("04.252.011/0001-10"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: "Sala 1"
            );
            var id = existing.Id;

            _individualRepo
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Individual?)null);

            _companyRepo
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            var request = new UpdatePersonRequest
            {
                AddressComplement = ""
            };

            _companyRepo
                .Setup(x => x.UpdateAsync(It.Is<Company>(c =>
                    c.AddressNumber == "10" &&
                    c.AddressComplement == null
                ), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _companyRepo
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            var result = await sut.UpdateAsync(id, request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(PersonType.Company, result!.Type);

            _individualRepo.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _individualRepo.VerifyNoOtherCalls();

            _companyRepo.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Exactly(2));
            _companyRepo.Verify(x => x.UpdateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
            _companyRepo.VerifyNoOtherCalls();

            _addressRepo.VerifyNoOtherCalls();
            _addressService.VerifyNoOtherCalls();
        }
    }
}
