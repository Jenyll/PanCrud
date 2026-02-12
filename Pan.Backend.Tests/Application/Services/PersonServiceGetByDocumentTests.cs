using Application.Dtos.Persons;
using Application.Ports;
using Application.Services;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Pan.Backend.Tests.Application.Services
{
    public sealed class PersonServiceGetByDocumentTests
    {
        private readonly Mock<IIndividualRepository> _individualRepo = new(MockBehavior.Strict);
        private readonly Mock<ICompanyRepository> _companyRepo = new(MockBehavior.Strict);
        private readonly Mock<IAddressRepository> _addressRepo = new(MockBehavior.Strict);
        private readonly Mock<IAddressService> _addressService = new(MockBehavior.Strict);

        private PersonService CreateSut()
            => new(_individualRepo.Object, _companyRepo.Object, _addressRepo.Object, _addressService.Object);

        [Fact]
        public async Task GetByDocumentAsync_ShouldReturnIndividual_WhenCpfMatches()
        {
            var sut = CreateSut();

            var individual = new Individual(
                name: "Maria",
                cpf: Cpf.From("529.982.247-25"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: null
            );

            _individualRepo
                .Setup(x => x.GetByCpfAsync("52998224725", It.IsAny<CancellationToken>()))
                .ReturnsAsync(individual);

            var result = await sut.GetByDocumentAsync("529.982.247-25", PersonType.Individual, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(PersonType.Individual, result!.Type);
            Assert.Equal("Maria", result.Name);
            Assert.Equal("52998224725", result.Cpf);

            _companyRepo.VerifyNoOtherCalls();
            _individualRepo.VerifyAll();
        }

        [Fact]
        public async Task GetByDocumentAsync_ShouldReturnCompany_WhenCnpjMatches()
        {
            var sut = CreateSut();

            var company = new Company(
                legalName: "ACME",
                cnpj: Cnpj.From("04.252.011/0001-10"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: null
            );

            _companyRepo
                .Setup(x => x.GetByCnpjAsync("04252011000110", It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);

            var result = await sut.GetByDocumentAsync("04.252.011/0001-10", PersonType.Company, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(PersonType.Company, result!.Type);
            Assert.Equal("ACME", result.LegalName);
            Assert.Equal("04252011000110", result.Cnpj);

            _individualRepo.VerifyNoOtherCalls();
            _companyRepo.VerifyAll();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("...")]
        public async Task GetByDocumentAsync_ShouldThrow_WhenDocumentIsInvalid(string input)
        {
            var sut = CreateSut();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                sut.GetByDocumentAsync(input, PersonType.Individual, CancellationToken.None));

            _individualRepo.VerifyNoOtherCalls();
            _companyRepo.VerifyNoOtherCalls();
        }
    }
}
