using Domain.Entities;
using Domain.ValueObjects;

namespace Pan.Backend.Tests.Domain.Entities
{
    public sealed class IndividualTests
    {
        [Fact]
        public void Ctor_ShouldCreate_WhenDataIsValid()
        {
            var addressId = Guid.NewGuid();

            var individual = new Individual(
                name: "Maria Silva",
                cpf: Cpf.From("529.982.247-25"),
                addressId: addressId,
                addressNumber: "123",
                addressComplement: "Apto 10"
            );

            Assert.NotEqual(Guid.Empty, individual.Id);
            Assert.Equal("Maria Silva", individual.Name);
            Assert.Equal("52998224725", individual.Cpf.Value);
            Assert.Equal(addressId, individual.AddressId);
            Assert.Equal("123", individual.AddressNumber);
            Assert.Equal("Apto 10", individual.AddressComplement);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Ctor_ShouldThrow_WhenNameIsBlank(string name)
        {
            Assert.ThrowsAny<Exception>(() =>
                new Individual(
                    name: name,
                    cpf: Cpf.From("529.982.247-25"),
                    addressId: Guid.NewGuid(),
                    addressNumber: "10",
                    addressComplement: null
                ));
        }

        [Fact]
        public void Rename_ShouldThrow_WhenNameBecomesBlank()
        {
            var individual = new Individual(
                name: "Maria",
                cpf: Cpf.From("529.982.247-25"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: null
            );

            Assert.ThrowsAny<Exception>(() => individual.Rename("   "));
        }

        [Fact]
        public void Rename_ShouldTrim()
        {
            var individual = new Individual(
                name: "Maria",
                cpf: Cpf.From("529.982.247-25"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: null
            );

            individual.Rename("  Maria Silva  ");

            Assert.Equal("Maria Silva", individual.Name);
        }

        [Fact]
        public void UpdateCpf_ShouldUpdate_WhenValid()
        {
            var individual = new Individual(
                name: "Maria",
                cpf: Cpf.From("529.982.247-25"),
                addressId: Guid.NewGuid(),
                addressNumber: "10",
                addressComplement: null
            );

            var newCpf = Cpf.From("390.533.447-05");
            individual.UpdateCpf(newCpf);

            Assert.Equal("39053344705", individual.Cpf.Value);
        }
    }
}
