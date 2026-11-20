using Phonebook.Data;
using Phonebook.Models;

namespace Phonebook.Importing;

public interface IDeviceContactSource
{
    Task<IReadOnlyList<Contact>> GetContactsAsync(CancellationToken cancellationToken = default);
}

public interface IContactPermissionGate
{
    Task<bool> EnsureCanReadContactsAsync();
}

/// <summary>
/// Сценарий кнопки импорта без привязки к странице MAUI.
/// Отказ в разрешении возвращает null. Пустой источник не вызывает сохранение.
/// Исключение источника пробрасывается: уже записанные контакты не откатываются, потому что запись ещё не начиналась.
/// </summary>
public sealed class ImportCoordinator
{
    public async Task<ContactImportResult?> RunAsync(
        IContactPermissionGate permissionGate,
        IDeviceContactSource source,
        ContactImportService importer,
        IContactStore store,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(permissionGate);
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(importer);
        ArgumentNullException.ThrowIfNull(store);

        if (!await permissionGate.EnsureCanReadContactsAsync().ConfigureAwait(false))
            return null;

        var contacts = await source.GetContactsAsync(cancellationToken).ConfigureAwait(false);
        if (contacts.Count == 0)
        {
            return new ContactImportResult
            {
                Completed = true,
                ImportedCount = 0,
                DuplicateCount = 0,
                SkippedCount = 0,
                WasCancelled = false
            };
        }

        return await importer.ImportAsync(contacts, store, cancellationToken).ConfigureAwait(false);
    }
}
