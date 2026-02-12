using Domain.ValueObjects;

namespace Pan.Backend.Tests.Domain.ValueObjects
{
    public sealed class CpfTests
    {
        [Fact]
        public void From_ShouldCreate_WhenCpfIsValid_WithMask()
        {
            var cpf = Cpf.From("529.982.247-25");
            Assert.Equal("52998224725", cpf.Value);
        }

        [Fact]
        public void From_ShouldThrow_WhenCpfHasInvalidCheckDigits()
        {
            Assert.Throws<ArgumentException>(() => Cpf.From("529.982.247-24"));
        }

        [Theory]
        [InlineData("11111111111")]
        [InlineData("00000000000")]
        public void From_ShouldThrow_WhenCpfIsAllSameDigits(string input)
        {
            Assert.Throws<ArgumentException>(() => Cpf.From(input));
        }

        [Theory]
        [InlineData("123")]
        [InlineData("123456789012")]
        public void From_ShouldThrow_WhenCpfHasInvalidLength(string input)
        {
            Assert.Throws<ArgumentException>(() => Cpf.From(input));
        }
    }
}
