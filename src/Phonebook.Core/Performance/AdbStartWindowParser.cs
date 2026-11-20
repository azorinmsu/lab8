namespace Phonebook.Performance;

public sealed record AdbStartMetrics(string Status, string? LaunchState, int? ThisTimeMs, int? TotalTimeMs, int? WaitTimeMs);

/// <summary>
/// Разбор текста `adb shell am start -W`. Сам по себе класс ничего не измеряет.
/// </summary>
public static class AdbStartWindowParser
{
    public static AdbStartMetrics Parse(string output)
    {
        if (output == null)
            throw new ArgumentNullException(nameof(output));

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var rawLine in output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            var line = rawLine.Trim();
            var separator = line.IndexOf(':');
            if (separator <= 0)
                continue;

            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim();
            map[key] = value;
        }

        map.TryGetValue("Status", out var status);
        map.TryGetValue("LaunchState", out var launchState);
        return new AdbStartMetrics(
            status ?? string.Empty,
            launchState,
            ReadInt(map, "ThisTime"),
            ReadInt(map, "TotalTime"),
            ReadInt(map, "WaitTime"));
    }

    private static int? ReadInt(Dictionary<string, string> map, string key)
    {
        if (!map.TryGetValue(key, out var value))
            return null;

        return int.TryParse(value, out var parsed) ? parsed : null;
    }
}
