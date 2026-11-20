namespace Phonebook.Contacts;

/// <summary>
/// Плоская запись, которую слой Android собирает из курсора ContactsContract.
/// Почтовый адрес платформенный читатель сейчас не заполняет.
/// </summary>
public sealed class RawDeviceContact
{
    public string? Id { get; init; }

    public string? DisplayName { get; init; }

    public string? PhoneNumber { get; init; }

    public string? Email { get; init; }

    public string? Address { get; init; }
}
