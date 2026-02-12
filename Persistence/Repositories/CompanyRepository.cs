using Domain.Entities;
using Persistence.EntityFramework;
using Application.Ports;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _db;
    public CompanyRepository(AppDbContext db) => _db = db;

    public Task<Company?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Companies.Include(c => c.Address).FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Company> CreateAsync(Company company, CancellationToken ct)
    {
        _db.Companies.Add(company);
        await _db.SaveChangesAsync(ct);
        return company;
    }

    public async Task UpdateAsync(Company company, CancellationToken ct)
    {
        _db.Companies.Update(company);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await _db.Companies.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;
        _db.Companies.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
    public Task<Company?> GetByCnpjAsync(string cnpj, CancellationToken ct)
    => _db.Companies
        .Include(c => c.Address)
        .FirstOrDefaultAsync(x => x.Cnpj.Value == cnpj, ct);
}