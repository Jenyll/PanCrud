using Application.Dtos.Addresses.Response;
using Domain.Entities;

namespace Application.Dtos.Addresses
{
    public static class AddressMapper
    {
        public static AddressResponse ToResponse(Address address) => new()
        {
            Id = address.Id,
            Cep = address.Cep.Value,
            Street = address.Street,
            Neighborhood = address.Neighborhood,
            City = address.City,
            State = address.State
        };
    }
}
