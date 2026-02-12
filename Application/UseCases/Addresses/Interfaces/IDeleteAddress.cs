namespace Application.UseCases.Addresses.Interfaces
{
    public interface IDeleteAddress
    {
        Task<bool> ExecuteAsync(Guid id, CancellationToken ct);
    }
}
