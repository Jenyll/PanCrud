using Application.Dtos.Addresses;

namespace Application.UseCases.Addresses.Interfaces;

public interface IGetAddress
{
    Task<AddressResponse?> ExecuteAsync(Guid id, CancellationToken ct);
}
