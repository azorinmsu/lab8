namespace Phonebook.Importing;

public sealed class ContactImportResult
{
    public int ImportedCount { get; init; }

    public int DuplicateCount { get; init; }

    public int SkippedCount { get; init; }

    public bool Completed { get; init; }

    /// <summary>
    /// Текущая реализация не прерывает цикл по CancellationToken, поэтому значение остаётся false.
    /// </summary>
    public bool WasCancelled { get; init; }
}
