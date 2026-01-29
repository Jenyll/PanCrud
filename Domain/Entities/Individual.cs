using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Individual : Person
{
    public string Name { get; private set; } = string.Empty;
    public Cpf Cpf { get; private set; } = default;

    // For EF Core
    private Individual() { }

    public Individual(string name, Cpf cpf, Guid addressId) : base(addressId)
    {
        Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
        Cpf = cpf;

        Validate();
    }

    public void Rename(string name)
    {
        Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
        Validate();
    }

    public void UpdateCpf(Cpf cpf)
    {
        Cpf = cpf;
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name é obrigatória.", nameof(Name));

        if (string.IsNullOrWhiteSpace(Cpf.Value))
            throw new ArgumentException("Cpf é obrigatório.", nameof(Cpf));
    }
}