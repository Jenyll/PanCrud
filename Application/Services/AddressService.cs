using Application.Dtos.Addresses.Request;
using Application.Dtos.Addresses.Response;
using Application.UseCases.Addresses.Interfaces;
using Application.Ports;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Services;

public sealed class AddressService : IAddressService
{
    private readonly ICreateAddress _create;
    private readonly IGetAddress _get;
    private readonly IUpdateAddress _update;
    private readonly IDeleteAddress _delete;
    private readonly IAddressRepository _repo;

    public AddressService(
        ICreateAddress create,
        IGetAddress get,
        IUpdateAddress update,
        IDeleteAddress delete,
        IAddressRepository repo)
    {
        _create = create;
        _get = get;
        _update = update;
        _delete = delete;
        _repo = repo;
    }
    public Task<AddressResponse> CreateAsync(CreateAddressRequest request, CancellationToken ct)
        => _create.ExecuteAsync(request, ct);

    public Task<AddressResponse?> GetByIdAsync(Guid id, CancellationToken ct)
        => _get.ExecuteAsync(id, ct);

    public Task<AddressResponse?> UpdateAsync(Guid id, UpdateAddressRequest request, CancellationToken ct)
        => _update.ExecuteAsync(id, request, ct);

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        => _delete.ExecuteAsync(id, ct);

    public async Task<bool> IsCepRegisteredAsync(string cep, CancellationToken ct)
    {
        var value = Cep.From(cep).Value;
        var existing = await _repo.GetByCepAsync(value, ct);
        return existing is not null;
    }
    public Task<Address?> GetAddressByCepAsync(string cep, CancellationToken ct)
    {
        var value = Cep.From(cep).Value;
        return _repo.GetByCepAsync(value, ct);
    }
    public async Task<Address> EnsureAddressExistsAsync(CreateAddressRequest request, CancellationToken ct)
    {
        var cepValue = Cep.From(request.Cep).Value;

        var existing = await _repo.GetByCepAsync(cepValue, ct);
        if (existing is not null) return existing;

        var createdDto = await _create.ExecuteAsync(request, ct);
        var createdEntity = await _repo.GetByIdAsync(createdDto.Id, ct);
        if (createdEntity is null)
            throw new InvalidOperationException("Endereço criado mas não encontrado no repositório.");

        return createdEntity;
    }
}