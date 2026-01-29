using Application.Dtos.Addresses;

namespace Application.UseCases.Addresses.Interfaces;

public interface ICreateAddress
{
    Task<AddressResponse> ExecuteAsync(CreateAddressRequest request, CancellationToken ct);
}
