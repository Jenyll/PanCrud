using Domain.Entities;
using Persistence.EntityFramework;
using Application.Ports;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _db;

        public AddressRepository(AppDbContext db) => _db = db;

        public Task<Address?> GetByIdAsync(Guid id, CancellationToken ct)
            => _db.Addresses.FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task<Address> CreateAsync(Address address, CancellationToken ct)
        {
            _db.Addresses.Add(address);
            await _db.SaveChangesAsync(ct);
            return address;
        }

        public async Task UpdateAsync(Address address, CancellationToken ct)
        {
            _db.Addresses.Update(address);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        {
            var address = await _db.Addresses.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (address is null) return false;

            _db.Addresses.Remove(address);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        // Implementação exigida pela interface IAddressRepository
        public Task<Address?> GetByCepAsync(string cep, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cep)) return Task.FromResult<Address?>(null);

            var normalized = Domain.ValueObjects.Cep.From(cep);

            return _db.Addresses.FirstOrDefaultAsync(x => x.Cep == normalized, ct);
        }
    }
}
