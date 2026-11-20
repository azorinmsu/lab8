using Phonebook.Models;

namespace Phonebook.Platform;

/// <summary>
/// MainActivity объявляет ConfigurationChanges, включая ориентацию и размер экрана,
/// поэтому поворот не пересоздаёт Activity. Отдельного сохранения формы при смерти процесса нет.
/// </summary>
public static class AndroidActivityPolicy
{
    public const int MinSdkVersion = 21;

    public const string ConfigurationChanges =
        "ScreenSize|Orientation|UiMode|ScreenLayout|SmallestScreenSize|Density";

    public const bool RecreatesActivityOnOrientationChange = false;

    public const bool PersistsFormAcrossProcessDeath = false;
}

/// <summary>
/// Поля формы карточки контакта. Страница хранит их в элементах ввода и сама снимок не пишет.
/// </summary>
public sealed class ContactFormSnapshot
{
    public string Name { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string? PhotoPath { get; init; }

    public static ContactFormSnapshot Capture(Contact? contact)
    {
        contact ??= new Contact();
        return new ContactFormSnapshot
        {
            Name = contact.Name ?? string.Empty,
            PhoneNumber = contact.PhoneNumber ?? string.Empty,
            Email = contact.Email ?? string.Empty,
            Address = contact.Address ?? string.Empty,
            Description = contact.Description ?? string.Empty,
            PhotoPath = contact.PhotoPath
        };
    }

    public Contact ApplyTo(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        contact.Name = Name;
        contact.PhoneNumber = PhoneNumber;
        contact.Email = Email;
        contact.Address = Address;
        contact.Description = Description;
        contact.PhotoPath = PhotoPath;
        return contact;
    }
}
