using Application.Dtos.Addresses;
using Application.Dtos.Addresses.Request;
using Application.Dtos.Addresses.Response;

namespace Application.UseCases.Addresses.Interfaces;

public interface ICreateAddress
{
    Task<AddressResponse> ExecuteAsync(CreateAddressRequest request, CancellationToken ct);
}
