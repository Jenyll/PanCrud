using Application.Dtos.Addresses.Request;
using Application.Dtos.Addresses.Response;
using Application.Dtos.Persons;
using Application.Dtos.Persons.Request;
using Application.Ports;
using Application.Services;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;
public sealed class PersonServiceCreateTests
{
    private readonly Mock<IIndividualRepository> _individualRepo = new(MockBehavior.Strict);
    private readonly Mock<ICompanyRepository> _companyRepo = new(MockBehavior.Strict);
    private readonly Mock<IAddressRepository> _addressRepo = new(MockBehavior.Strict);
    private readonly Mock<IAddressService> _addressService = new(MockBehavior.Strict);

    private PersonService CreateSut()
        => new(_individualRepo.Object, _companyRepo.Object, _addressRepo.Object, _addressService.Object);

    [Fact]
    public async Task CreateAsync_ShouldCreateIndividual_WhenAddressIdProvided()
    {
        var sut = CreateSut();
        var addressId = Guid.NewGuid();

        var request = new CreatePersonRequest
        {
            Type = PersonType.Individual,
            Name = "Maria Silva",
            Cpf = "529.982.247-25",
            AddressId = addressId,
            AddressNumber = "123",
            AddressComplement = "Apto 10"
        };

        _individualRepo
            .Setup(x => x.CreateAsync(It.Is<Individual>(i =>
                i.Name == "Maria Silva" &&
                i.Cpf.Value == "52998224725" &&
                i.AddressId == addressId &&
                i.AddressNumber == "123" &&
                i.AddressComplement == "Apto 10"
            ), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Individual i, CancellationToken _) => i);

        _individualRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Individual?)null);

        var result = await sut.CreateAsync(request, CancellationToken.None);

        Assert.Equal(PersonType.Individual, result.Type);
        Assert.Equal("Maria Silva", result.Name);
        Assert.Equal("52998224725", result.Cpf);
        Assert.Equal(addressId, result.AddressId);

        _addressRepo.VerifyNoOtherCalls();
        _addressService.VerifyNoOtherCalls();
        _companyRepo.VerifyNoOtherCalls();
        _individualRepo.VerifyAll();
    }

    [Fact]
    public async Task CreateAsync_ShouldUseExistingAddressFromDb_WhenAddressProvidedAndFoundByCep()
    {
        var sut = CreateSut();

        var request = new CreatePersonRequest
        {
            Type = PersonType.Individual,
            Name = "Maria Silva",
            Cpf = "529.982.247-25",
            AddressId = null,
            Address = new CreateAddressRequest { Cep = "60181-225" },
            AddressNumber = "123",
            AddressComplement = null
        };

        Address existingAddress = null!;
        _addressRepo
            .Setup(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                existingAddress = new Address(
                    Cep.From("60181-225"),
                    street: "Rua X",
                    neighborhood: "Bairro",
                    city: "Fortaleza",
                    state: "CE"
                );
                return existingAddress;
            });

        _individualRepo
            .Setup(x => x.CreateAsync(It.Is<Individual>(i =>
                i.AddressId == existingAddress.Id &&
                i.AddressNumber == "123" &&
                i.AddressComplement == null
            ), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Individual i, CancellationToken _) => i);

        _individualRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Individual?)null);

        var result = await sut.CreateAsync(request, CancellationToken.None);

        Assert.Equal(PersonType.Individual, result.Type);
        Assert.Equal(existingAddress.Id, result.AddressId);

        _addressService.VerifyNoOtherCalls();
        _companyRepo.VerifyNoOtherCalls();
        _addressRepo.VerifyAll();
        _individualRepo.VerifyAll();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAddressViaAddressService_WhenAddressNotFoundInDb()
    {
        var sut = CreateSut();
        var createdAddressId = Guid.NewGuid();

        var request = new CreatePersonRequest
        {
            Type = PersonType.Individual,
            Name = "Maria Silva",
            Cpf = "529.982.247-25",
            AddressId = null,
            Address = new CreateAddressRequest { Cep = "60181-225" },
            AddressNumber = "123",
            AddressComplement = null
        };

        _addressRepo
            .Setup(x => x.GetByCepAsync("60181225", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Address?)null);

        _addressService
            .Setup(x => x.CreateAsync(It.Is<CreateAddressRequest>(a => a.Cep == "60181-225"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AddressResponse
            {
                Id = createdAddressId,
                Cep = "60181225",
                Street = "Rua Pascoal de Castro Alves",
                Neighborhood = "Vicente Pinzon",
                City = "Fortaleza",
                State = "CE"
            });

        _individualRepo
            .Setup(x => x.CreateAsync(It.Is<Individual>(i =>
                i.AddressId == createdAddressId &&
                i.AddressNumber == "123"
            ), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Individual i, CancellationToken _) => i);

        _individualRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Individual?)null);

        var result = await sut.CreateAsync(request, CancellationToken.None);

        Assert.Equal(PersonType.Individual, result.Type);
        Assert.Equal(createdAddressId, result.AddressId);

        _addressRepo.VerifyAll();
        _addressService.VerifyAll();
        _individualRepo.VerifyAll();
        _companyRepo.VerifyNoOtherCalls();
    }
}