using Application.Dtos.Addresses;

namespace Application.UseCases.Addresses;

public interface IGetAddressUseCase
{
    Task<AddressResponse?> ExecuteAsync(Guid id, CancellationToken ct);
}
