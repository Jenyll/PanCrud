using Application.Dtos.Addresses;

namespace Application.UseCases.Addresses.Interfaces;

public interface IUpdateAddress
{
    Task<AddressResponse?> ExecuteAsync(Guid id, UpdateAddressRequest request, CancellationToken ct);
}
