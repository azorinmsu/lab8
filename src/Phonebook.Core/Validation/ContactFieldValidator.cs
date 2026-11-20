using System.Net.Mail;

namespace Phonebook.Validation;

/// <summary>
/// Фактические проверки полей. Сохранение контакта в интерфейсе эти проверки не вызывает.
/// Email проверяется через <see cref="MailAddress"/> и равенство <c>Address</c> исходной строке.
/// Это не реализация RFC 5322.
/// </summary>
public static class ContactFieldValidator
{
    public const bool SaveEnforcesRequiredFields = false;

    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    /// <summary>
    /// Звонок возможен, если строка номера не пустая. Формат дополнительно не проверяется.
    /// </summary>
    public static bool HasDialablePhone(string? phone) => !string.IsNullOrEmpty(phone);
}
