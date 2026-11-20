using Phonebook.Data;

namespace Phonebook.TestSupport;

public sealed class TempSqlite : IAsyncDisposable
{
    public TempSqlite()
    {
        DirectoryPath = Path.Combine(Path.GetTempPath(), "phonebook-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(DirectoryPath);
        DatabasePath = Path.Combine(DirectoryPath, "contacts.db3");
        Repository = new ContactRepository(DatabasePath);
    }

    public string DirectoryPath { get; }

    public string DatabasePath { get; }

    public ContactRepository Repository { get; }

    public async ValueTask DisposeAsync()
    {
        await Repository.DisposeAsync();
        TryDelete(DirectoryPath);
    }

    public static void TryDelete(string directory)
    {
        try
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
        catch (IOException)
        {
        }
    }
}
