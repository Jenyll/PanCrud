using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Company : Person
{
    public string LegalName { get; private set; } = string.Empty;

    public Cnpj Cnpj { get; private set; }

    // Para EF Core
    private Company() { }

    public Company(string legalName, Cnpj cnpj, Guid addressId) : base(addressId)
    {
        LegalName = legalName?.Trim() ?? throw new ArgumentNullException(nameof(legalName), "Razão social é obrigatória.");
        Cnpj = cnpj;

        Validate();
    }

    public void Rename(string legalName)
    {
        LegalName = legalName?.Trim() ?? throw new ArgumentNullException(nameof(legalName), "Razão social é obrigatória.");
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(LegalName))
            throw new ArgumentException("Razão social é obrigatória.", nameof(LegalName));
    }
}