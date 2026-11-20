namespace Phonebook.Performance;

public sealed record MeasurementRow(IReadOnlyList<string> Columns, IReadOnlyList<string> Values);

public static class MeasurementCsvWriter
{
    public static async Task WriteAsync(string path, IReadOnlyList<string> columns, IEnumerable<IReadOnlyList<string>> rows, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(columns);

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        await using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync(string.Join(",", columns.Select(Escape))).ConfigureAwait(false);
        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (row.Count != columns.Count)
                throw new InvalidOperationException("Число значений в строке не совпадает с заголовком.");

            await writer.WriteLineAsync(string.Join(",", row.Select(Escape))).ConfigureAwait(false);
        }
    }

    public static string Escape(string? value)
    {
        var text = value ?? string.Empty;
        if (text.Contains('"') || text.Contains(',') || text.Contains('\n'))
            return "\"" + text.Replace("\"", "\"\"") + "\"";

        return text;
    }
}
