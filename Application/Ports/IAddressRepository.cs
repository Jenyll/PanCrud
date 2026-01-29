using Domain.Entities;

namespace Application.Ports;

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Address> CreateAsync(Address address, CancellationToken ct);
    Task UpdateAsync(Address address, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<Address?> GetByCepAsync(string cep, CancellationToken ct);
}
