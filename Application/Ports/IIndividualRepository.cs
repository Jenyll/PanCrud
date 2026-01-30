using Domain.Entities;

namespace Application.Ports;

public interface IIndividualRepository
{
    Task<Individual?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Individual> CreateAsync(Individual individual, CancellationToken ct);
    Task UpdateAsync(Individual individual, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<Individual?> GetByCpfAsync(string cpf, CancellationToken ct);
}