using Application.Dtos.Addresses.Response;

namespace Application.Dtos.Persons.Response;

public sealed class PersonResponse
{
    public Guid Id { get; set; }
    public PersonType Type { get; set; }

    // PF
    public string? Name { get; set; }
    public string? Cpf { get; set; }

    // PJ
    public string? LegalName { get; set; }
    public string? Cnpj { get; set; }

    public Guid AddressId { get; set; }
    public AddressResponse? Address { get; set; }
}