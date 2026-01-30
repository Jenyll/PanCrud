using Application.Dtos.Addresses;
using Application.Dtos.Addresses.Request;
using Application.Dtos.Addresses.Response;

namespace Application.Services;

public interface IAddressService
{
    Task<AddressResponse> CreateAsync(CreateAddressRequest request, CancellationToken ct);
    Task<AddressResponse?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<AddressResponse?> UpdateAsync(Guid id, UpdateAddressRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<AddressResponse?> GetByCepAsync(string cep, CancellationToken ct);
    Task<AddressLookupResponse?> LookupByCepAsync(string cep, CancellationToken ct);
}