namespace Domain.Entities;

public abstract class Person
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public Guid AddressId { get; protected set; }
    public string AddressNumber { get; protected set; } = string.Empty;
    public string? AddressComplement { get; protected set; }
    public Address? Address { get; protected set; }
    protected Person() { }

    protected Person(Guid addressId, string addressNumber, string? addressComplement)
    {
        if (addressId == Guid.Empty)
            throw new ArgumentException("AddressId não pode ser vazio", nameof(addressId));

        AddressId = addressId;
        SetAddressDetails(addressNumber, addressComplement);
    }

    public void UpdateAddress(Guid addressId)
    {
        if (addressId == Guid.Empty)
            throw new ArgumentException("AddressId não pode ser vazio", nameof(addressId));

        AddressId = addressId;
    }

    public void SetAddressDetails(string number, string? complement)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Número do endereço é obrigatório.", nameof(number));

        AddressNumber = number.Trim();
        AddressComplement = string.IsNullOrWhiteSpace(complement) ? null : complement.Trim();
    }
}
