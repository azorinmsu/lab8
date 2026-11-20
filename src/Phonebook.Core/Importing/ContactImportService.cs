using Phonebook.Data;
using Phonebook.Models;
using Phonebook.Phone;

namespace Phonebook.Importing;

/// <summary>
/// Импорт списка контактов в хранилище.
/// Дубликат — совпадение <see cref="PhoneNumberNormalizer.NormalizeKey"/> с уже сохранённым или ранее принятым номером.
/// Каждая вставка выполняется отдельно, без общей транзакции.
/// Переданный <see cref="CancellationToken"/> не проверяется.
/// </summary>
public class ContactImportService
{
    public async Task<ContactImportResult> ImportAsync(
        IReadOnlyList<Contact> deviceContacts,
        IContactStore store,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(deviceContacts);
        ArgumentNullException.ThrowIfNull(store);
        _ = cancellationToken;

        var existingContacts = await store.GetContactsAsync().ConfigureAwait(false);
        var existingNormalizedPhones = new HashSet<string>();
        foreach (var contact in existingContacts)
        {
            var normalized = PhoneNumberNormalizer.NormalizeKey(contact.PhoneNumber);
            if (!string.IsNullOrEmpty(normalized))
                existingNormalizedPhones.Add(normalized);
        }

        var importedCount = 0;
        var duplicateCount = 0;
        var skippedCount = 0;

        foreach (var contact in deviceContacts)
        {
            var normalizedPhone = PhoneNumberNormalizer.NormalizeKey(contact.PhoneNumber);
            if (string.IsNullOrEmpty(normalizedPhone))
            {
                skippedCount++;
                continue;
            }

            if (!existingNormalizedPhones.Contains(normalizedPhone))
            {
                await store.SaveContactAsync(contact).ConfigureAwait(false);
                existingNormalizedPhones.Add(normalizedPhone);
                importedCount++;
            }
            else
            {
                duplicateCount++;
            }
        }

        return new ContactImportResult
        {
            ImportedCount = importedCount,
            DuplicateCount = duplicateCount,
            SkippedCount = skippedCount,
            Completed = true,
            WasCancelled = false
        };
    }
}
