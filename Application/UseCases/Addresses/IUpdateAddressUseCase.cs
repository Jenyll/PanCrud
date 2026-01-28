using Application.Dtos.Addresses;

namespace Application.UseCases.Addresses;

public interface IUpdateAddressUseCase
{
    Task<AddressResponse?> ExecuteAsync(Guid id, UpdateAddressRequest request, CancellationToken ct);
}
