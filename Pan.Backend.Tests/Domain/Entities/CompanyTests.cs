using Domain.Entities;
using Domain.ValueObjects;

namespace Pan.Backend.Tests.Domain.Entities
{
    public sealed class CompanyTests
    {
        [Fact]
        public void Ctor_ShouldCreate_WhenDataIsValid()
        {
            var addressId = Guid.NewGuid();

            var company = new Company(
                legalName: "ACME LTDA",
                cnpj: Cnpj.From("04.252.011/0001-10"),
                addressId: addressId,
                addressNumber: "500",
                addressComplement: "Sala 301"
            );

            Assert.NotEqual(Guid.Empty, company.Id);
            Assert.Equal("ACME LTDA", company.LegalName);
            Assert.Equal("04252011000110", company.Cnpj.Value);
            Assert.Equal(addressId, company.AddressId);
            Assert.Equal("500", company.AddressNumber);
            Assert.Equal("Sala 301", company.AddressComplement);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Ctor_ShouldThrow_WhenLegalNameIsBlank(string legalName)
        {
            Assert.ThrowsAny<Exception>(() =>
                new Company(
                    legalName: legalName,
                    cnpj: Cnpj.From("04.252.011/0001-10"),
                    addressId: Guid.NewGuid(),
                    addressNumber: "10",
                    addressComplement: null
                ));
        }

        [Fact]
        public void Rename_ShouldTrim()
        {
            var company = new Company(
                legalName: "ACME",
                cnpj: Cnpj.From("04.252.011/0001-10"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: null
            );

            company.Rename("  ACME LTDA  ");

            Assert.Equal("ACME LTDA", company.LegalName);
        }

        [Fact]
        public void Rename_ShouldThrow_WhenBlank()
        {
            var company = new Company(
                legalName: "ACME",
                cnpj: Cnpj.From("04.252.011/0001-10"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: null
            );

            Assert.ThrowsAny<Exception>(() => company.Rename("   "));
        }

        [Fact]
        public void UpdateCnpj_ShouldUpdate()
        {
            var company = new Company(
                legalName: "ACME",
                cnpj: Cnpj.From("04.252.011/0001-10"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: null
            );

            var newCnpj = Cnpj.From("40.688.134/0001-61");
            company.UpdateCnpj(newCnpj);

            Assert.Equal("40688134000161", company.Cnpj.Value);
        }
    }
}
