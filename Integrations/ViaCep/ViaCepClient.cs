using Application.Dtos.ViaCep;
using System.Net.Http.Json;

namespace Integrations.ViaCep
{
    public sealed class ViaCepClient : IViaCepClient
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
                Complement = dto.complemento ?? "",
                Unit = dto.unidade ?? "",
                Neighborhood = dto.bairro ?? "",
                City = dto.localidade ?? "",
                State = dto.uf ?? "",
                StateName = dto.estado ?? "",
                Region = dto.regiao ?? "",
                Ibge = dto.ibge ?? "",
                Gia = dto.gia ?? "",
                Ddd = dto.ddd ?? "",
                Siafi = dto.siafi ?? "",

                Error = dto.erro ?? false
            };
        }
        private sealed class ViaCepResponse
        {
            public string? cep { get; set; }
            public string? logradouro { get; set; }
            public string? complemento { get; set; }
            public string? unidade { get; set; }
            public string? bairro { get; set; }
            public string? localidade { get; set; }
            public string? uf { get; set; }
            public string? estado { get; set; }
            public string? regiao { get; set; }
            public string? ibge { get; set; }
            public string? gia { get; set; }
            public string? ddd { get; set; }
            public string? siafi { get; set; }
            public bool? erro { get; set; }
        }
    }
}
