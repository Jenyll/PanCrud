using Domain.ValueObjects;

namespace Pan.Backend.Tests.Domain.ValueObjects
{
    public sealed class CepTests
    {
        [Fact]
        public void From_ShouldCreate_WhenCepIsValid_WithHyphen()
        {
            var cep = Cep.From("60181-225");
            Assert.Equal("60181225", cep.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("123")]
        [InlineData("123456789")]
        public void From_ShouldThrow_WhenCepHasInvalidLength(string input)
        {
            Assert.Throws<ArgumentException>(() => Cep.From(input));
        }

        [Theory]
        [InlineData("00000000")]
        [InlineData("11111111")]
        public void From_ShouldThrow_WhenCepIsAllSameDigits(string input)
        {
            Assert.Throws<ArgumentException>(() => Cep.From(input));
        }
    }
}
