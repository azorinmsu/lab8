using Phonebook.Models;

namespace Phonebook.Importing;

/// <summary>
/// Контакты для окружения Testing. Это вымышленные записи, не данные пользователя.
/// </summary>
public sealed class FakeDeviceContactSource : IDeviceContactSource
{
    private readonly IReadOnlyList<Contact> _contacts;

    public FakeDeviceContactSource(IReadOnlyList<Contact>? contacts = null)
    {
        _contacts = contacts ?? CreateSamples();
    }

    public Task<IReadOnlyList<Contact>> GetContactsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_contacts);
    }

    public static IReadOnlyList<Contact> CreateSamples() =>
    [
        new Contact
        {
            Name = "Учебный контакт Альфа",
            PhoneNumber = "+7 (900) 000-00-01",
            Email = "alpha@example.test",
            Description = "Фиктивная запись окружения Testing"
        },
        new Contact
        {
            Name = "Учебный контакт Бета",
            PhoneNumber = "+7 (900) 000-00-02",
            Email = "beta@example.test",
            Description = "Фиктивная запись окружения Testing"
        }
    ];
}

public sealed class AllowAllContactPermissionGate : IContactPermissionGate
{
    public Task<bool> EnsureCanReadContactsAsync() => Task.FromResult(true);
}

public sealed class DenyContactPermissionGate : IContactPermissionGate
{
    public Task<bool> EnsureCanReadContactsAsync() => Task.FromResult(false);
}
