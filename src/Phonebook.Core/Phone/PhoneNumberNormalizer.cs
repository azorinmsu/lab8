namespace Phonebook.Phone;

/// <summary>
/// Две исторически разные нормализации номеров.
/// <see cref="NormalizeStored"/> используется при чтении контактов Android.
/// <see cref="NormalizeKey"/> используется при сравнении номеров во время импорта и в списке.
/// </summary>
public static class PhoneNumberNormalizer
{
    /// <summary>
    /// Алгоритм <c>DeviceContactsService</c>: если в строке нет цифр, возвращается исходное значение.
    /// </summary>
    public static string NormalizeStored(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        var digitsOnly = Digits(phone);

        if (digitsOnly.StartsWith("89") && digitsOnly.Length == 11)
            return "7" + digitsOnly[1..];

        if (digitsOnly.StartsWith("79") && digitsOnly.Length == 11)
            return digitsOnly;

        if (digitsOnly.Length == 10)
            return "7" + digitsOnly;

        return digitsOnly.Length > 0 ? digitsOnly : phone;
    }

    /// <summary>
    /// Алгоритм сравнения в <c>MainPage</c>: возвращаются только цифры (пустая строка, если цифр нет).
    /// </summary>
    public static string NormalizeKey(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        var digitsOnly = Digits(phone);

        if (digitsOnly.StartsWith("89") && digitsOnly.Length == 11)
            return "7" + digitsOnly[1..];

        if (digitsOnly.StartsWith("79") && digitsOnly.Length == 11)
            return digitsOnly;

        if (digitsOnly.Length == 10)
            return "7" + digitsOnly;

        return digitsOnly;
    }

    public static string FormatForDisplay(string? normalizedPhone)
    {
        if (string.IsNullOrEmpty(normalizedPhone))
            return string.Empty;

        if (normalizedPhone.StartsWith('7') && normalizedPhone.Length == 11)
        {
            return $"+7 ({normalizedPhone.Substring(1, 3)}) {normalizedPhone.Substring(4, 3)}-{normalizedPhone.Substring(7, 2)}-{normalizedPhone.Substring(9, 2)}";
        }

        return normalizedPhone;
    }

    private static string Digits(string phone) => new(phone.Where(char.IsDigit).ToArray());
}
