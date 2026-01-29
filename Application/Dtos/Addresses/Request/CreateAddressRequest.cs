namespace Application.Dtos.Addresses.Request
{
    public sealed class CreateAddressRequest
    {
        public string Cep { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string? Complement { get; set; }
    }
}
