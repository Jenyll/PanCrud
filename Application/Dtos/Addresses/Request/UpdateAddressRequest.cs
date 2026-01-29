namespace Application.Dtos.Addresses.Request
{
    public sealed class UpdateAddressRequest
    {
        public string? Number { get; set; }
        public string? Complement { get; set; }

        // Opcional: permitir trocar CEP e reconsultar ViaCEP
        public string? Cep { get; set; }
    }
}
