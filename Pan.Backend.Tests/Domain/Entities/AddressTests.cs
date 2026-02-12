using Domain.Entities;
using Domain.ValueObjects;

namespace Pan.Backend.Tests.Domain.Entities
{
    public sealed class AddressTests
    {
        [Fact]
        public void SetUserDetails_ShouldSetNumberAndComplement_WhenValid()
        {
            var address = new Address(
                cep: Cep.From("60181-225"),
                street: "Rua X",
                neighborhood: "Bairro",
                city: "Fortaleza",
                state: "CE"
            );

            address.SetUserDetails(" 123 ", " Apto 10 ");

            Assert.Equal("123", address.Number);
            Assert.Equal("Apto 10", address.Complement);
        }

        [Fact]
        public void SetUserDetails_ShouldSetComplementNull_WhenEmptyOrWhitespace()
        {
            var address = new Address(
                cep: Cep.From("60181-225"),
                street: "Rua X",
                neighborhood: "Bairro",
                city: "Fortaleza",
                state: "CE"
            );

            address.SetUserDetails("123", "   ");

            Assert.Equal("123", address.Number);
            Assert.Null(address.Complement);
        }

        [Fact]
        public void ClearUserDetails_ShouldNullNumberAndComplement()
        {
            var address = new Address(
                cep: Cep.From("60181-225"),
                street: "Rua X",
                neighborhood: "Bairro",
                city: "Fortaleza",
                state: "CE"
            );

            address.SetUserDetails("123", "Apto 10");
            address.ClearUserDetails();

            Assert.Null(address.Number);
            Assert.Null(address.Complement);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenStateIsInvalid()
        {
            Assert.Throws<ArgumentException>(() =>
                new Address(
                    cep: Cep.From("60181-225"),
                    street: "Rua X",
                    neighborhood: "Bairro",
                    city: "Fortaleza",
                    state: "C" // inválido, precisa 2 letras
                ));
        }

        [Fact]
        public void UpdateFromViaCep_ShouldThrow_WhenStateIsInvalid()
        {
            var address = new Address(
                cep: Cep.From("60181-225"),
                street: "Rua X",
                neighborhood: "Bairro",
                city: "Fortaleza",
                state: "CE"
            );

            Assert.Throws<ArgumentException>(() =>
                address.UpdateFromViaCep(
                    street: "Rua Y",
                    neighborhood: "Centro",
                    city: "Fortaleza",
                    state: "C" // inválido
                ));
        }
    }
}
