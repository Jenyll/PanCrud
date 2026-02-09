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

        if (digits.All(c => c == digits[0]))
            throw new ArgumentException("CNPJ inválido.", nameof(input));

        if (!IsValidCnpjDigits(digits))
            throw new ArgumentException("CNPJ inválido.", nameof(input));

        return new Cnpj(digits);
    }
    private static bool IsValidCnpjDigits(string digits)
    {
        int[] w1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] w2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        int sum1 = 0;
        for (int i = 0; i < 12; i++)
            sum1 += (digits[i] - '0') * w1[i];

        int mod1 = sum1 % 11;
        int dv1 = mod1 < 2 ? 0 : 11 - mod1;

        if ((digits[12] - '0') != dv1)
            return false;

        int sum2 = 0;
        for (int i = 0; i < 13; i++)
            sum2 += (digits[i] - '0') * w2[i];

        int mod2 = sum2 % 11;
        int dv2 = mod2 < 2 ? 0 : 11 - mod2;

        return (digits[13] - '0') == dv2;
    }

    public override string ToString() => Value;
}       