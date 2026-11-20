namespace Phonebook.Platform;

public enum CallAttemptResult
{
    OpenDialer,
    DialerUnavailable,
    PhoneMissing
}

public enum MailAttemptResult
{
    OpenClient,
    InvalidEmail,
    EmailMissing,
    ClientMissing
}

/// <summary>
/// Реальные сценарии связи. Отдельного SMS Intent в приложении нет.
/// Наличие SIM-карты код не проверяет.
/// </summary>
public static class CommunicationIntents
{
    public const string DialAction = "android.intent.action.DIAL";

    public const string SendToAction = "android.intent.action.SENDTO";

    public const bool AppImplementsSms = false;

    public const bool SimCardIsChecked = false;

    public const string ActivityNotFoundMessage =
        "Не найден почтовый клиент. Установите Gmail, Outlook или другой клиент.";

    public const string DialerUnavailableMessage = "На этом устройстве нет возможности звонить";

    public const string PhoneMissingMessage = "Номер телефона не указан";

    public static string? BuildMailtoUri(string? email) =>
        string.IsNullOrWhiteSpace(email) ? null : "mailto:" + email.Trim();

    public static string? BuildDialUri(string? phone) =>
        string.IsNullOrWhiteSpace(phone) ? null : "tel:" + phone;

    public static CallAttemptResult DecideCall(bool dialerSupported, string? phone)
    {
        if (string.IsNullOrEmpty(phone))
            return CallAttemptResult.PhoneMissing;

        if (!dialerSupported)
            return CallAttemptResult.DialerUnavailable;

        return CallAttemptResult.OpenDialer;
    }
}
