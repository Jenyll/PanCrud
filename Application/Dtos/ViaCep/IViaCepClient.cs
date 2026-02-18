namespace Application.Dtos.ViaCep
{
    public interface IViaCepClient
    {
        Task<ViaCepAddress?> GetAsync(string cep, CancellationToken ct);
    }
}
