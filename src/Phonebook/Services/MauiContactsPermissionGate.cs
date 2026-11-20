using Phonebook.Importing;

namespace Phonebook.Services;

public sealed class MauiContactsPermissionGate : IContactPermissionGate
{
    public async Task<bool> EnsureCanReadContactsAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.ContactsRead>();

        return status == PermissionStatus.Granted;
    }
}
