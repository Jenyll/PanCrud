using Application.Dtos.Addresses;
using Application.Dtos.Addresses.Response;

namespace Application.UseCases.Addresses.Interfaces;

public interface IGetAddress
{
    Task<AddressResponse?> ExecuteAsync(Guid id, CancellationToken ct);
}
