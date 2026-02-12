using Domain.Entities;
using Domain.ValueObjects;

namespace Pan.Backend.Tests.Domain.Entities
{
    public sealed class PersonTests
    {
        [Fact]
        public void Ctor_ShouldThrow_WhenAddressIdIsEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                new Individual(
                    name: "Maria",
                    cpf: Cpf.From("529.982.247-25"),
                    addressId: Guid.Empty,
                    addressNumber: "10",
                    addressComplement: null
                ));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void SetAddressDetails_ShouldThrow_WhenNumberIsEmpty(string number)
        {
            var p = new Individual(
                name: "Maria",
                cpf: Cpf.From("529.982.247-25"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: null
            );

            Assert.Throws<ArgumentException>(() => p.SetAddressDetails(number, "Apto 1"));
        }

        [Fact]
        public void SetAddressDetails_ShouldTrim_AndNullifyComplement_WhenBlank()
        {
            var p = new Individual(
                name: "Maria",
                cpf: Cpf.From("529.982.247-25"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: "x"
            );

            p.SetAddressDetails("  200  ", "   ");

            Assert.Equal("200", p.AddressNumber);
            Assert.Null(p.AddressComplement);
        }

        [Fact]
        public void UpdateAddress_ShouldThrow_WhenAddressIdIsEmpty()
        {
            var p = new Individual(
                name: "Maria",
                cpf: Cpf.From("529.982.247-25"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: null
            );

            Assert.Throws<ArgumentException>(() => p.UpdateAddress(Guid.Empty));
        }

        [Fact]
        public void UpdateAddress_ShouldUpdate_WhenValid()
        {
            var p = new Individual(
                name: "Maria",
                cpf: Cpf.From("529.982.247-25"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: null
            );

            var newAddressId = Guid.NewGuid();
            p.UpdateAddress(newAddressId);

            Assert.Equal(newAddressId, p.AddressId);
        }
    }
}
