using System.Text.Json;
using Phonebook.Unsplash;

namespace Phonebook.Services;

public static class UnsplashKeyLoader
{
    public static string? Load()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable(UnsplashService.AccessKeyEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
            return fromEnvironment.Trim();

        foreach (var path in CandidateFiles())
        {
            if (!File.Exists(path))
                continue;

            try
            {
                using var document = JsonDocument.Parse(File.ReadAllText(path));
                if (!document.RootElement.TryGetProperty("UnsplashAccessKey", out var property))
                    continue;

                var value = property.GetString();
                if (string.IsNullOrWhiteSpace(value) || value.Contains("PUT_", StringComparison.Ordinal))
                    continue;

                return value.Trim();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Не удалось прочитать локальные настройки Unsplash: {ex.Message}");
            }
        }

        return null;
    }

    private static IEnumerable<string> CandidateFiles()
    {
        yield return Path.Combine(AppContext.BaseDirectory, "appsettings.local.json");
        yield return Path.Combine(Directory.GetCurrentDirectory(), "appsettings.local.json");
    }
}
