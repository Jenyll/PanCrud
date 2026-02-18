using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pan.Backend.Tests.Domain.ValueObjects
{
    public sealed class CnpjTests
    {
        [Fact]
        public void From_ShouldCreate_WhenCnpjIsValid_WithMask()
        {
            var cnpj = Cnpj.From("04.252.011/0001-10");
            Assert.Equal("04252011000110", cnpj.Value);
        }

        [Fact]
        public void From_ShouldThrow_WhenCnpjHasInvalidCheckDigits()
        {
            Assert.Throws<ArgumentException>(() => Cnpj.From("04.252.011/0001-11"));
        }

        [Theory]
        [InlineData("00000000000000")]
        [InlineData("11111111111111")]
        public void From_ShouldThrow_WhenCnpjIsAllSameDigits(string input)
        {
            Assert.Throws<ArgumentException>(() => Cnpj.From(input));
        }

        [Theory]
        [InlineData("123")]
        [InlineData("123456789012345")]
        public void From_ShouldThrow_WhenCnpjHasInvalidLength(string input)
        {
            Assert.Throws<ArgumentException>(() => Cnpj.From(input));
        }
    }
}
