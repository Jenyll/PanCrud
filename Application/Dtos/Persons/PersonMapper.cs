using Application.Dtos.Persons.Response;
using Domain.Entities;
using Application.Dtos.Addresses;

namespace Application.Dtos.Persons;

public static class PersonMapper
{
    public static PersonResponse FromIndividual(Individual i) => new()
    {
        Id = i.Id,
        Type = PersonType.Individual,
        Name = i.Name,
        Cpf = i.Cpf.Value,
        AddressId = i.AddressId,
        Address = i.Address is null ? null : AddressMapper.ToResponse(i.Address)
    };

    public static PersonResponse FromCompany(Company c) => new()
    {
        Id = c.Id,
        Type = PersonType.Company,
        LegalName = c.LegalName,
        Cnpj = c.Cnpj.Value,
        AddressId = c.AddressId,
        Address = c.Address is null ? null : AddressMapper.ToResponse(c.Address)
    };
}