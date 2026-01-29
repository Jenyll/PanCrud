using System.Net.Http.Json;

namespace Integrations.ViaCep
{
    internal class ViaCepClient : IViaCepClient
    {
        private readonly HttpClient _http;

        public ViaCepClient(HttpClient http) => _http = http;

        public async Task<ViaCepAddress?> GetAsync(string cep, CancellationToken ct)
        {
            // ViaCEP aceita /ws/{cep}/json/
            var path = $"ws/{cep}/json/";

            var dto = await _http.GetFromJsonAsync<ViaCepResponse>(path, cancellationToken: ct);
            if (dto is null) return null;

            return new ViaCepAddress
            {
                Cep = dto.cep ?? "",
                Street = dto.logradouro ?? "",
                Neighborhood = dto.bairro ?? "",
                City = dto.localidade ?? "",
                State = dto.uf ?? "",
                Error = dto.erro ?? false
            };
        }

        // DTO interno do provider (mantém a sujeira aqui dentro)
        private sealed class ViaCepResponse
        {
            public string? cep { get; set; }
            public string? logradouro { get; set; }
            public string? bairro { get; set; }
            public string? localidade { get; set; }
            public string? uf { get; set; }
            public bool? erro { get; set; }
        }
    }
}
