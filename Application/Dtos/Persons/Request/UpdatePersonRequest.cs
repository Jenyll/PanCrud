using Application.Dtos.Addresses.Request;

namespace Application.Dtos.Persons.Request;

public sealed class UpdatePersonRequest
{
    public string? Name { get; set; }
    public string? Cpf { get; set; }

    public string? LegalName { get; set; }
    public string? Cnpj { get; set; }

    // Endereço: trocar por Id existente ou fornecer nova informação para criação/atualização do endereço
    public Guid? AddressId { get; set; }
    public CreateAddressRequest? Address { get; set; }
}