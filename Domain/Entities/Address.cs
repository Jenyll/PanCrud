using Domain.ValueObjects;

namespace Domain.Entities;

public class Address
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Cep Cep { get; private set; }

    public string Street { get; private set; } = string.Empty;
    public string Number { get; private set; } = string.Empty;
    public string? Complement { get; private set; }
    public string Neighborhood { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;

    // For EF Core
    private Address() { }

    public Address(
        Cep cep,
        string street,
        string number,
        string? complement,
        string neighborhood,
        string city,
        string state)
    {
        Cep = cep;
        Street = street?.Trim() ?? throw new ArgumentNullException(nameof(street));
        Number = number?.Trim() ?? throw new ArgumentNullException(nameof(number));
        Complement = string.IsNullOrWhiteSpace(complement) ? null : complement.Trim();
        Neighborhood = neighborhood?.Trim() ?? throw new ArgumentNullException(nameof(neighborhood));
        City = city?.Trim() ?? throw new ArgumentNullException(nameof(city));
        State = state?.Trim() ?? throw new ArgumentNullException(nameof(state));

        Validate();
    }

    public void UpdateFromViaCep(string street, string neighborhood, string city, string state)
    {
        Street = street?.Trim() ?? string.Empty;
        Neighborhood = neighborhood?.Trim() ?? string.Empty;
        City = city?.Trim() ?? string.Empty;
        State = state?.Trim() ?? string.Empty;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(State) || State.Length != 2)
            throw new ArgumentException("State must be a 2-letter UF code (e.g., 'SP').", nameof(State));
    }
    public void UpdateCepAndFromViaCep(Cep cep, string street, string neighborhood, string city, string state)
    {
        Cep = cep;
        UpdateFromViaCep(street, neighborhood, city, state);
    }
}