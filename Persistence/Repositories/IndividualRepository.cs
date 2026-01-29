using Domain.Entities;
using Persistence.EntityFramework;
using Application.Ports;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class IndividualRepository : IIndividualRepository
{
    private readonly AppDbContext _db;
    public IndividualRepository(AppDbContext db) => _db = db;

    public Task<Individual?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Individuals.Include(i => i.Address).FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Individual> CreateAsync(Individual individual, CancellationToken ct)
    {
        _db.Individuals.Add(individual);
        await _db.SaveChangesAsync(ct);
        return individual;
    }

    public async Task UpdateAsync(Individual individual, CancellationToken ct)
    {
        _db.Individuals.Update(individual);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await _db.Individuals.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;
        _db.Individuals.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}