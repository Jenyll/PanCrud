using Application.Ports;
using Application.UseCases.Addresses.Implement;
using Moq;

namespace Pan.Backend.Tests.Application.UseCases.Addresses
{
    public sealed class DeleteAddressUseCaseTests
    {
        private readonly Mock<IAddressRepository> _repo = new(MockBehavior.Strict);

        [Fact]
        public async Task ExecuteAsync_ShouldReturnTrue_WhenRepositoryDeletes()
        {
            var id = Guid.NewGuid();

            _repo
                .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var sut = new DeleteAddress(_repo.Object);

            var result = await sut.ExecuteAsync(id, CancellationToken.None);

            Assert.True(result);
            _repo.Verify(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _repo.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFalse_WhenRepositoryDoesNotDelete()
        {
            var id = Guid.NewGuid();

            _repo
                .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var sut = new DeleteAddress(_repo.Object);

            var result = await sut.ExecuteAsync(id, CancellationToken.None);

            Assert.False(result);
            _repo.Verify(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _repo.VerifyNoOtherCalls();
        }
    }
}
