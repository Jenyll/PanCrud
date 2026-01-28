namespace Domain.ValueObjects;

public readonly record struct Cpf(string Value)
{
    public static Cpf From(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("CPF is required.", nameof(input));

        var digits = new string(input.Where(char.IsDigit).ToArray());

        if (digits.Length != 11)
            throw new ArgumentException("CPF must have 11 digits.", nameof(input));

        return new Cpf(digits);
    }

    public override string ToString() => Value;
}