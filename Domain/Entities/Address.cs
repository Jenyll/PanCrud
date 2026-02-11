using Domain.ValueObjects;

namespace Domain.Entities;

public class Address
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Cep Cep { get; private set; }

    // Dados do ViaCEP
    public string Street { get; private set; } = string.Empty;
    public string Neighborhood { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;

    // Dados informados pelo usuário (não vêm do ViaCEP)
    public string? Number { get; private set; }
    public string? Complement { get; private set; }

    // For EF Core
    private Address() { }

    /// <summary>
    /// Endereço base (lookup por CEP): não exige número.
    /// </summary>
    public Address(Cep cep, string street, string neighborhood, string city, string state)
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
    /// Atualiza CEP + dados ViaCEP.
    /// </summary>
    public void UpdateCepAndFromViaCep(Cep cep, string street, string neighborhood, string city, string state)
    {
        Cep = cep;
        UpdateFromViaCep(street, neighborhood, city, state);
    }

    /// <summary>
    /// Preenche/atualiza os dados informados pelo usuário.
    /// Regra: Number obrigatório se você quiser “endereço completo”.
    /// </summary>
    public void SetUserDetails(string number, string? complement)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Número é obrigatório.", nameof(number));

        Number = number.Trim();
        Complement = string.IsNullOrWhiteSpace(complement) ? null : complement.Trim();
    }

    /// <summary>
    /// Permite limpar os dados do usuário (se fizer sentido no seu CRUD).
    /// </summary>
    public void ClearUserDetails()
    {
        Number = null;
        Complement = null;
    }

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
