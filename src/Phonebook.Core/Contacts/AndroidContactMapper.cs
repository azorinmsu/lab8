using Phonebook.Models;
using Phonebook.Phone;

namespace Phonebook.Contacts;

/// <summary>
/// Правила преобразования контактов устройства, перенесённые из <c>DeviceContactsService</c>.
/// Для одного идентификатора сохраняется последняя телефонная строка.
/// Дубликаты по <see cref="PhoneNumberNormalizer.NormalizeKey"/> отбрасываются: остаётся первый.
/// </summary>
public static class AndroidContactMapper
{
    public const string ImportedDescription = "Импортирован с устройства";

    public static IReadOnlyList<Contact> Map(IEnumerable<RawDeviceContact>? rawContacts)
    {
        var byId = new Dictionary<string, Contact>();
        var emails = new Dictionary<string, string>();

        foreach (var raw in rawContacts ?? [])
        {
            if (!string.IsNullOrWhiteSpace(raw.Email) && !string.IsNullOrWhiteSpace(raw.Id))
                emails[raw.Id] = raw.Email.Trim();

            if (string.IsNullOrWhiteSpace(raw.DisplayName) || string.IsNullOrWhiteSpace(raw.PhoneNumber))
                continue;

            var normalized = PhoneNumberNormalizer.NormalizeStored(raw.PhoneNumber);
            if (string.IsNullOrWhiteSpace(normalized))
                continue;

            if (string.IsNullOrWhiteSpace(raw.Id))
                continue;

            byId[raw.Id] = new Contact
            {
                Name = raw.DisplayName.Trim(),
                PhoneNumber = PhoneNumberNormalizer.FormatForDisplay(normalized),
                Email = string.Empty,
                Description = ImportedDescription,
                Address = raw.Address ?? string.Empty
            };
        }

        foreach (var pair in emails)
        {
            if (byId.TryGetValue(pair.Key, out var contact))
                contact.Email = pair.Value;
        }

        var unique = new Dictionary<string, Contact>();
        foreach (var contact in byId.Values)
        {
            var key = PhoneNumberNormalizer.NormalizeKey(contact.PhoneNumber);
            if (!string.IsNullOrWhiteSpace(key) && !unique.ContainsKey(key))
                unique[key] = contact;
        }

        return unique.Values.ToList();
    }
}
