using Application.Ports;
using Application.UseCases.Addresses.Implement;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Pan.Backend.Tests.Application.UseCases.Addresses
{
    public sealed class GetAddressUseCaseTests
    {
        private readonly Mock<IAddressRepository> _repo = new(MockBehavior.Strict);

        [Fact]
        public async Task ExecuteAsync_ShouldReturnNull_WhenAddressNotFound()
        {
            var id = Guid.NewGuid();

            _repo
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Address?)null);

            var sut = new GetAddress(_repo.Object);

            var result = await sut.ExecuteAsync(id, CancellationToken.None);

            Assert.Null(result);
            _repo.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _repo.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnResponse_WhenFound()
        {
            var address = new Address(
                cep: Cep.From("60181-225"),
                street: "Rua X",
                neighborhood: "Vicente Pinzon",
                city: "Fortaleza",
                state: "CE"
            );

            _repo
                .Setup(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(address);

            var sut = new GetAddress(_repo.Object);

            var result = await sut.ExecuteAsync(address.Id, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(address.Id, result!.Id);
            Assert.Equal("60181225", result.Cep.Replace("-", ""));
            Assert.Equal("Rua X", result.Street);
            Assert.Equal("Vicente Pinzon", result.Neighborhood);
            Assert.Equal("Fortaleza", result.City);
            Assert.Equal("CE", result.State);

            _repo.Verify(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()), Times.Once);
            _repo.VerifyNoOtherCalls();
        }
    }
}
