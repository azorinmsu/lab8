namespace Phonebook.Configuration;

public enum PhonebookEnvironment
{
    Development,
    Testing
}

/// <summary>
/// Окружение приложения. Не совпадает с конфигурацией сборки Debug/Release.
/// </summary>
public static class PhonebookEnvironmentSelector
{
    public const string VariableName = "PHONEBOOK_ENVIRONMENT";

    public static PhonebookEnvironment Resolve(string? configuredValue = null)
    {
        var raw = configuredValue ?? Environment.GetEnvironmentVariable(VariableName);
        if (Enum.TryParse<PhonebookEnvironment>(raw, ignoreCase: true, out var parsed))
            return parsed;

        return PhonebookEnvironment.Development;
    }

    public static PhonebookEnvironment Current => Resolve();
}
