using Application.Dtos.Addresses.Request;
using Application.Dtos.ViaCep;
using Application.Ports;
using Application.UseCases.Addresses.Implement;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Pan.Backend.Tests.Application.UseCases.Addresses
{
    public sealed class CreateAddressUseCaseTests
    {
        private readonly Mock<IViaCepClient> _viaCep = new(MockBehavior.Strict);
        private readonly Mock<IAddressRepository> _repo = new(MockBehavior.Strict);

        private CreateAddress CreateSut() => new(_viaCep.Object, _repo.Object);

        [Fact]
        public async Task ExecuteAsync_ShouldReturnExisting_WhenAddressAlreadyExistsInDatabase()
        {
            // Arrange
            var existing = new Address(
                Cep.From("60181-225"),
                street: "Rua Já Salva",
                neighborhood: "Vicente Pinzon",
                city: "Fortaleza",
                state: "CE"
            );

            _repo
                .Setup(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            var sut = CreateSut();

            var request = new CreateAddressRequest
            {
                Cep = "60181-225"
            };

            // Act
            var result = await sut.ExecuteAsync(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existing.Id, result!.Id);
            Assert.Equal("60181225", result.Cep);
            Assert.Equal("Rua Já Salva", result.Street);
            Assert.Equal("Fortaleza", result.City);
            Assert.Equal("CE", result.State);

            _repo.Verify(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()), Times.Once);
            _viaCep.VerifyNoOtherCalls();
            _repo.VerifyNoOtherCalls(); 
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCreate_WhenNotExistsAndViaCepReturnsValidData()
        {
            // Arrange
            _repo
                .Setup(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Address?)null);

            _viaCep
                .Setup(x => x.GetAsync("60181225", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ViaCepAddress
                {
                    Cep = "60181-225",
                    Street = "Rua Pascoal de Castro Alves",
                    Neighborhood = "Vicente Pinzon",
                    City = "Fortaleza",
                    State = "CE",
                    StateName = "Ceará",
                    Region = "Nordeste",
                    Ibge = "2304400",
                    Ddd = "85",
                    Siafi = "1389",
                    Error = false
                });

            _repo
                .Setup(x => x.CreateAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Address a, CancellationToken _) => a);

            var sut = CreateSut();

            var request = new CreateAddressRequest
            {
                Cep = "60181-225"
            };

            // Act
            var result = await sut.ExecuteAsync(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result!.Id);
            Assert.Equal("60181225", result.Cep);
            Assert.Equal("Rua Pascoal de Castro Alves", result.Street);
            Assert.Equal("Vicente Pinzon", result.Neighborhood);
            Assert.Equal("Fortaleza", result.City);
            Assert.Equal("CE", result.State);

            _repo.Verify(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()), Times.Once);
            _viaCep.Verify(x => x.GetAsync("60181225", It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(x => x.CreateAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Once);

            _repo.VerifyNoOtherCalls();
            _viaCep.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenNotExistsAndViaCepReturnsError()
        {
            // Arrange
            _repo
                .Setup(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Address?)null);

            _viaCep
                .Setup(x => x.GetAsync("60181225", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ViaCepAddress { Error = true });

            var sut = CreateSut();

            var request = new CreateAddressRequest
            {
                Cep = "60181-225"
            };

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                sut.ExecuteAsync(request, CancellationToken.None));

            _repo.Verify(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()), Times.Once);
            _viaCep.Verify(x => x.GetAsync("60181225", It.IsAny<CancellationToken>()), Times.Once);

            _repo.Verify(x => x.CreateAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Never);
            _repo.VerifyNoOtherCalls();
            _viaCep.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenCepIsInvalid()
        {
            var sut = CreateSut();

            var request = new CreateAddressRequest
            {
                Cep = "00000000" // inválido pelo VO
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                sut.ExecuteAsync(request, CancellationToken.None));

            _repo.VerifyNoOtherCalls();
            _viaCep.VerifyNoOtherCalls();
        }
    }
}
