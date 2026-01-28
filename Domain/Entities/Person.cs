namespace Domain.Entities;

public abstract class Person
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public Guid AddressId { get; protected set; }

    // Optional navigation (EF Core can use it, but the domain does not depend on EF)
    public Address? Address { get; protected set; }

    // For EF Core
    protected Person() { }

    protected Person(Guid addressId)
    {
        if (addressId == Guid.Empty)
            throw new ArgumentException("AddressId não pode ser vazio", nameof(addressId));

        AddressId = addressId;
    }

    public void UpdateAddress(Guid addressId)
    {
        if (addressId == Guid.Empty)
            throw new ArgumentException("AddressId não pode ser vazio", nameof(addressId));

        AddressId = addressId;
    }
}