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

        if (digits.All(c => c == digits[0]))
            throw new ArgumentException("CPF inválido.", nameof(input));

        if (!IsValidCpfDigits(digits))
            throw new ArgumentException("CPF inválido.", nameof(input));


        return new Cpf(digits);
    }
    private static bool IsValidCpfDigits(string digits)
    {
        // Primeiro dígito verificador
        int sum1 = 0;
        for (int i = 0; i < 9; i++)
            sum1 += (digits[i] - '0') * (10 - i);

        int mod1 = sum1 % 11;
        int dv1 = mod1 < 2 ? 0 : 11 - mod1;

        if ((digits[9] - '0') != dv1)
            return false;

        // Segundo dígito verificador
        int sum2 = 0;
        for (int i = 0; i < 10; i++)
            sum2 += (digits[i] - '0') * (11 - i);

        int mod2 = sum2 % 11;
        int dv2 = mod2 < 2 ? 0 : 11 - mod2;

        return (digits[10] - '0') == dv2;
    }

    public override string ToString() => Value;
}