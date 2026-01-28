namespace Application.UseCases.Addresses
{
    public interface IDeleteAddressUseCase
    {
        Task<bool> ExecuteAsync(Guid id, CancellationToken ct);
    }
}
