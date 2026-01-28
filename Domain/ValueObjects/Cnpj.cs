namespace Domain.ValueObjects;

public readonly record struct Cnpj(string Value)
{
    public static Cnpj From(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("CNPJ is required.", nameof(input));

        var digits = new string(input.Where(char.IsDigit).ToArray());

        if (digits.Length != 14)
            throw new ArgumentException("CNPJ must have 14 digits.", nameof(input));

        return new Cnpj(digits);
    }

    public override string ToString() => Value;
}       