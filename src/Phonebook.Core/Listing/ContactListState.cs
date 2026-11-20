using Phonebook.Models;

namespace Phonebook.Listing;

/// <summary>
/// Правила списка на главной странице. Это не отдельная ViewModel: в приложении её нет.
/// Фильтр смотрит имя, телефон и email. Адрес и описание не участвуют.
/// Отдельной сортировки нет: сохраняется порядок источника.
/// Повторяющиеся элементы источника не удаляются.
/// </summary>
public sealed class ContactListState
{
    public const bool AppliesSort = false;

    public IReadOnlyList<Contact> Filter(IReadOnlyList<Contact>? source, string? searchTerm)
    {
        var items = source ?? [];
        var term = searchTerm?.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(term))
            return items.ToList();

        return items.Where(contact =>
                (contact.Name?.ToLower().Contains(term) ?? false) ||
                (contact.PhoneNumber?.ToLower().Contains(term) ?? false) ||
                (contact.Email?.ToLower().Contains(term) ?? false))
            .ToList();
    }

    public string FormatCount(int visibleCount, bool filtered) =>
        filtered ? $"Найдено: {visibleCount} контактов" : $"{visibleCount} контактов";
}
