using Domain.Entities;

namespace Application.Ports;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Company> CreateAsync(Company company, CancellationToken ct);
    Task UpdateAsync(Company company, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}