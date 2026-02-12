using Application.Ports;
using Application.Services;
using Moq;

namespace Pan.Backend.Tests.Application.Services
{
    public sealed class PersonServiceDeleteTests
    {
        private readonly Mock<IIndividualRepository> _individualRepo = new(MockBehavior.Strict);
        private readonly Mock<ICompanyRepository> _companyRepo = new(MockBehavior.Strict);
        private readonly Mock<IAddressRepository> _addressRepo = new(MockBehavior.Strict);
        private readonly Mock<IAddressService> _addressService = new(MockBehavior.Strict);

        private PersonService CreateSut()
            => new(_individualRepo.Object, _companyRepo.Object, _addressRepo.Object, _addressService.Object);

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenIndividualDeleted()
        {
            var sut = CreateSut();
            var id = Guid.NewGuid();

            _individualRepo
                .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await sut.DeleteAsync(id, CancellationToken.None);

            Assert.True(result);

            _companyRepo.VerifyNoOtherCalls();
            _addressRepo.VerifyNoOtherCalls();
            _addressService.VerifyNoOtherCalls();
            _individualRepo.VerifyAll();
        }

        [Fact]
        public async Task DeleteAsync_ShouldTryCompany_WhenIndividualNotDeleted()
        {
            var sut = CreateSut();
            var id = Guid.NewGuid();

            _individualRepo
                .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _companyRepo
                .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await sut.DeleteAsync(id, CancellationToken.None);

            Assert.True(result);

            _addressRepo.VerifyNoOtherCalls();
            _addressService.VerifyNoOtherCalls();
            _companyRepo.VerifyAll();
            _individualRepo.VerifyAll();
        }
    }
}
