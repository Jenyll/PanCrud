namespace Application.Dtos.ViaCep
{
    public sealed class ViaCepAddress
    {
        public string Cep { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;      
        public string Neighborhood { get; set; } = string.Empty;  
        public string City { get; set; } = string.Empty;         
        public string State { get; set; } = string.Empty;          
        public bool Error { get; set; }                          
    }
}
