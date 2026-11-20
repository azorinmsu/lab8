using Phonebook.Models;
using SQLite;

namespace Phonebook.Performance;

public sealed record InsertRun(string Strategy, int Contacts, bool Warmup, int Iteration, long ElapsedMilliseconds);

/// <summary>
/// Сравнение вставки контактов: по одной строке, в явной транзакции и пакетом InsertAll.
/// Прогрев помечается и не должен смешиваться с измеряемыми запусками при анализе.
/// </summary>
public static class SqliteInsertExperiment
{
    public static readonly string[] Strategies = ["single", "transaction", "batch"];

    public static async Task<IReadOnlyList<InsertRun>> RunAsync(
        int contacts,
        int warmupRuns,
        int measuredRuns,
        string workingDirectory,
        CancellationToken cancellationToken = default)
    {
        if (contacts <= 0)
            throw new ArgumentOutOfRangeException(nameof(contacts));
        if (warmupRuns < 0 || measuredRuns < 0)
            throw new ArgumentOutOfRangeException(nameof(measuredRuns));

        Directory.CreateDirectory(workingDirectory);
        var sample = ContactDataSetFactory.Create(contacts);
        var runs = new List<InsertRun>();

        foreach (var strategy in Strategies)
        {
            for (var i = 0; i < warmupRuns; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                runs.Add(await MeasureAsync(strategy, sample, warmup: true, i, workingDirectory).ConfigureAwait(false));
            }

            for (var i = 0; i < measuredRuns; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                runs.Add(await MeasureAsync(strategy, sample, warmup: false, i, workingDirectory).ConfigureAwait(false));
            }
        }

        return runs;
    }

    public static async Task WriteCsvAsync(string path, IReadOnlyList<InsertRun> runs, CancellationToken cancellationToken = default)
    {
        var rows = runs.Select(run => (IReadOnlyList<string>)
        [
            run.Strategy,
            run.Contacts.ToString(),
            run.Warmup ? "warmup" : "measured",
            run.Iteration.ToString(),
            run.ElapsedMilliseconds.ToString()
        ]);

        await MeasurementCsvWriter.WriteAsync(
            path,
            ["strategy", "contacts", "phase", "iteration", "elapsed_ms"],
            rows,
            cancellationToken).ConfigureAwait(false);
    }

    private static async Task<InsertRun> MeasureAsync(
        string strategy,
        IReadOnlyList<Contact> sample,
        bool warmup,
        int iteration,
        string workingDirectory)
    {
        var path = Path.Combine(workingDirectory, $"{strategy}-{warmup}-{iteration}-{Guid.NewGuid():N}.db3");
        var connection = new SQLiteAsyncConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
        try
        {
            await connection.CreateTableAsync<Contact>().ConfigureAwait(false);
            var started = System.Diagnostics.Stopwatch.GetTimestamp();
            switch (strategy)
            {
                case "single":
                    foreach (var contact in Clone(sample))
                        await connection.InsertAsync(contact).ConfigureAwait(false);
                    break;
                case "transaction":
                    await connection.RunInTransactionAsync(sqlite =>
                    {
                        foreach (var contact in Clone(sample))
                            sqlite.Insert(contact);
                    }).ConfigureAwait(false);
                    break;
                case "batch":
                    await connection.InsertAllAsync(Clone(sample)).ConfigureAwait(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, "Неизвестная стратегия.");
            }

            var elapsed = System.Diagnostics.Stopwatch.GetElapsedTime(started);
            var stored = await connection.Table<Contact>().CountAsync().ConfigureAwait(false);
            if (stored != sample.Count)
                throw new InvalidOperationException($"Стратегия {strategy} сохранила {stored} из {sample.Count}.");

            return new InsertRun(strategy, sample.Count, warmup, iteration, (long)elapsed.TotalMilliseconds);
        }
        finally
        {
            await connection.CloseAsync().ConfigureAwait(false);
            TryDelete(path);
        }
    }

    private static List<Contact> Clone(IReadOnlyList<Contact> sample) =>
        sample.Select(contact => new Contact
        {
            Name = contact.Name,
            PhoneNumber = contact.PhoneNumber,
            Email = contact.Email,
            Address = contact.Address,
            Description = contact.Description,
            PhotoPath = contact.PhotoPath
        }).ToList();

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (IOException)
        {
        }
    }
}
