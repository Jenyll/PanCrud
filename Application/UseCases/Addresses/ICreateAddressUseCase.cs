using Application.Dtos.Addresses;

namespace Application.UseCases.Addresses;

public interface ICreateAddressUseCase
{
    Task<AddressResponse> ExecuteAsync(CreateAddressRequest request, CancellationToken ct);
}
