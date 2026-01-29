using Application.Dtos.Addresses;
using Application.UseCases.Addresses.Interfaces;

namespace Application.Services;

public sealed class AddressService : IAddressService
{
    private readonly ICreateAddress _create;
    private readonly IGetAddress _get;
    private readonly IUpdateAddress _update;
    private readonly IDeleteAddress _delete;

    public AddressService(
        ICreateAddress create,
        IGetAddress get,
        IUpdateAddress update,
        IDeleteAddress delete)
    {
        _create = create;
        _get = get;
        _update = update;
        _delete = delete;
    }

    public Task<AddressResponse> CreateAsync(CreateAddressRequest request, CancellationToken ct)
        => _create.ExecuteAsync(request, ct);

    public Task<AddressResponse?> GetByIdAsync(Guid id, CancellationToken ct)
        => _get.ExecuteAsync(id, ct);

    public Task<AddressResponse?> UpdateAsync(Guid id, UpdateAddressRequest request, CancellationToken ct)
        => _update.ExecuteAsync(id, request, ct);

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        => _delete.ExecuteAsync(id, ct);
}