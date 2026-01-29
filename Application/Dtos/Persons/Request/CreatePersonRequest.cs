using Application.Dtos.Addresses.Request;

namespace Application.Dtos.Persons.Request;

public sealed class CreatePersonRequest
{
    public PersonType Type { get; set; }

    // PF
    public string? Name { get; set; }
    public string? Cpf { get; set; }

    // PJ
    public string? LegalName { get; set; }
    public string? Cnpj { get; set; }

    // Endereço: pode enviar AddressId (usar endereço existente) ou objeto para criação (cep, number, complement)
    public Guid? AddressId { get; set; }
    public CreateAddressRequest? Address { get; set; }
}