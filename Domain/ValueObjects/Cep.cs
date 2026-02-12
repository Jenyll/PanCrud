namespace Domain.ValueObjects;

public readonly record struct Cep(string Value)
{
    public static Cep From(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("CEP is required.", nameof(input));

        var digits = new string(input.Where(char.IsDigit).ToArray());

        if (digits.Length != 8)
            throw new ArgumentException("CEP must have 8 digits.", nameof(input));

        if (digits.All(c => c == digits[0]))
            throw new ArgumentException("CEP inválido.", nameof(input));

        return new Cep(digits);
    }

    public override string ToString() => Value;
}