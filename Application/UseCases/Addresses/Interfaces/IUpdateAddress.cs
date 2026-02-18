using Application.Dtos.Addresses;
using Application.Dtos.Addresses.Request;
using Application.Dtos.Addresses.Response;

namespace Application.UseCases.Addresses.Interfaces;

public interface IUpdateAddress
{
    Task<AddressResponse?> ExecuteAsync(Guid id, UpdateAddressRequest request, CancellationToken ct);
}
