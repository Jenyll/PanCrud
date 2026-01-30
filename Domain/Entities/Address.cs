using Domain.ValueObjects;

namespace Domain.Entities;

public class Address
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Cep Cep { get; private set; }    
    public string Street { get; private set; } = string.Empty;
    public string Neighborhood { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;

    // Dados "do usuário" (somente no cadastro da Person)
    public string? Number { get; private set; } 
    public string? Complement { get; private set; }

    private Address() { }

    /// <summary>
    /// Endereço base (lookup por CEP): não exige número.
    /// </summary>
    public Address(
        Cep cep,
        string street,
        string neighborhood,
        string city,
        string state)
    {
        Cep = cep;

        Street = NormalizeRequired(street, nameof(street));
        Neighborhood = NormalizeRequired(neighborhood, nameof(neighborhood));
        City = NormalizeRequired(city, nameof(city));
        State = NormalizeRequired(state, nameof(state));

        ValidateBasics();
    }

    /// <summary>
    /// Define/atualiza dados retornados do ViaCEP para o CEP atual.
    /// </summary>
    public void UpdateFromViaCep(string street, string neighborhood, string city, string state)
    {
        Street = NormalizeRequired(street, nameof(street));
        Neighborhood = NormalizeRequired(neighborhood, nameof(neighborhood));
        City = NormalizeRequired(city, nameof(city));
        State = NormalizeRequired(state, nameof(state));

        ValidateBasics();
    }

    /// <summary>
    /// Atualiza CEP + dados ViaCEP (se você precisar "trocar" o CEP do endereço).
    /// </summary>
    public void UpdateCepAndFromViaCep(Cep cep, string street, string neighborhood, string city, string state)
    {
        Cep = cep;
        UpdateFromViaCep(street, neighborhood, city, state);
    }

    /// <summary>
    /// Preenche os dados que o usuário informa (no cadastro da Person).
    /// </summary>   

    private static string NormalizeRequired(string value, string paramName)
        => string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"{paramName} é obrigatório.", paramName)
            : value.Trim();

    private void ValidateBasics()
    {
        if (State.Length != 2)
            throw new ArgumentException("UF deve ter 2 letras (ex: SP, CE).", nameof(State));
    }
}
