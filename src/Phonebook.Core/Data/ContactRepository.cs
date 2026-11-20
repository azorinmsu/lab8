using System.Globalization;
using Phonebook.Models;
using SQLite;

namespace Phonebook.Data;

/// <summary>
/// Доступ к SQLite. Инициализация ленивая и ожидается вызывающим кодом:
/// исходный <c>async void Initialize</c> заменён, чтобы тесты не зависели от гонки.
/// Поиск загружает все строки и фильтрует их в памяти.
/// <see cref="GetContactAsync"/> по-прежнему подставляет целочисленный идентификатор в текст SQL.
/// </summary>
public class ContactRepository : IContactStore, IAsyncDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private SQLiteAsyncConnection? _database;

    public ContactRepository(string databasePath)
    {
        if (string.IsNullOrWhiteSpace(databasePath))
            throw new ArgumentException("Путь к базе не задан.", nameof(databasePath));

        DatabasePath = databasePath;
    }

    public string DatabasePath { get; }

    public async Task InitializeAsync()
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_database != null)
                return;

            var directory = Path.GetDirectoryName(DatabasePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            _database = new SQLiteAsyncConnection(DatabasePath, LocalDataPlacement.OpenFlags);
            await _database.CreateTableAsync<Contact>().ConfigureAwait(false);
            await AddMissingColumnsAsync().ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<List<Contact>> GetContactsAsync()
    {
        var database = await DatabaseAsync().ConfigureAwait(false);
        return await database.Table<Contact>().ToListAsync().ConfigureAwait(false);
    }

    public async Task<Contact?> GetContactAsync(int id)
    {
        var database = await DatabaseAsync().ConfigureAwait(false);
        var query = $"SELECT * FROM Contact WHERE Id = {id}";
        var result = await database.QueryAsync<Contact>(query).ConfigureAwait(false);
        return result.FirstOrDefault();
    }

    public async Task<int> SaveContactAsync(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        var database = await DatabaseAsync().ConfigureAwait(false);
        if (contact.Id != 0)
            return await database.UpdateAsync(contact).ConfigureAwait(false);

        return await database.InsertAsync(contact).ConfigureAwait(false);
    }

    public async Task<int> DeleteContactAsync(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        var database = await DatabaseAsync().ConfigureAwait(false);
        return await database.DeleteAsync(contact).ConfigureAwait(false);
    }

    public async Task<List<Contact>> SearchContactsAsync(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetContactsAsync().ConfigureAwait(false);

        var allContacts = await GetContactsAsync().ConfigureAwait(false);
        var result = new List<Contact>();
        var compareInfo = CultureInfo.CurrentCulture.CompareInfo;

        foreach (var contact in allContacts)
        {
            var matches =
                compareInfo.IndexOf(contact.Name ?? "", searchTerm, CompareOptions.IgnoreCase) >= 0 ||
                compareInfo.IndexOf(contact.PhoneNumber ?? "", searchTerm, CompareOptions.IgnoreCase) >= 0 ||
                compareInfo.IndexOf(contact.Email ?? "", searchTerm, CompareOptions.IgnoreCase) >= 0 ||
                compareInfo.IndexOf(contact.Address ?? "", searchTerm, CompareOptions.IgnoreCase) >= 0 ||
                compareInfo.IndexOf(contact.Description ?? "", searchTerm, CompareOptions.IgnoreCase) >= 0;

            if (matches)
                result.Add(contact);
        }

        return result.DistinctBy(contact => contact.Id).ToList();
    }

    public async ValueTask DisposeAsync()
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_database != null)
            {
                await _database.CloseAsync().ConfigureAwait(false);
                _database = null;
            }
        }
        finally
        {
            _gate.Release();
        }

        _gate.Dispose();
    }

    private async Task<SQLiteAsyncConnection> DatabaseAsync()
    {
        await InitializeAsync().ConfigureAwait(false);
        return _database ?? throw new InvalidOperationException("База не инициализирована.");
    }

    private async Task AddMissingColumnsAsync()
    {
        if (_database == null)
            return;

        try
        {
            var tableInfo = await _database.GetTableInfoAsync("Contact").ConfigureAwait(false);
            var columns = tableInfo.Select(column => column.Name).ToList();

            if (!columns.Contains("Email"))
                await _database.ExecuteAsync("ALTER TABLE Contact ADD COLUMN Email TEXT").ConfigureAwait(false);

            if (!columns.Contains("Address"))
                await _database.ExecuteAsync("ALTER TABLE Contact ADD COLUMN Address TEXT").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при добавлении колонок: {ex.Message}");
        }
    }
}
