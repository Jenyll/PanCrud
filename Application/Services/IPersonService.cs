using Application.Dtos.Persons;
using Application.Dtos.Persons.Request;
using Application.Dtos.Persons.Response;

namespace Application.Services;

public interface IPersonService
{
    Task<PersonResponse> CreateAsync(CreatePersonRequest request, CancellationToken ct);
    Task<PersonResponse?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PersonResponse?> UpdateAsync(Guid id, UpdatePersonRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<PersonResponse?> GetByDocumentAsync(string number, PersonType type, CancellationToken ct);
}