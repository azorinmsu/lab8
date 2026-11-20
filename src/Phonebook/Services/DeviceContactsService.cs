using Phonebook.Contacts;
using Phonebook.Importing;
using Phonebook.Models;
using Contact = Phonebook.Models.Contact;

#if ANDROID
using Android.Provider;
#endif

namespace Phonebook.Services;

public class DeviceContactsService : IDeviceContactSource
{
#if ANDROID
    private readonly Android.Content.Context? _context;
#endif

    public DeviceContactsService()
    {
#if ANDROID
        _context = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
#endif
    }

    public async Task<IReadOnlyList<Contact>> GetContactsAsync(CancellationToken cancellationToken = default)
    {
#if ANDROID
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            return AndroidContactMapper.Map(ReadRawContacts());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка импорта контактов: {ex.Message}");
            return [];
        }
#else
        await Task.CompletedTask;
        return [];
#endif
    }

#if ANDROID
    private List<RawDeviceContact> ReadRawContacts()
    {
        var raw = new List<RawDeviceContact>();
        if (_context == null)
            return raw;

        var phoneProjection = new[]
        {
            ContactsContract.Contacts.InterfaceConsts.Id,
            ContactsContract.Contacts.InterfaceConsts.DisplayName,
            ContactsContract.CommonDataKinds.Phone.Number
        };

        var phoneCursor = _context.ContentResolver?.Query(
            ContactsContract.CommonDataKinds.Phone.ContentUri,
            phoneProjection,
            null,
            null,
            $"{ContactsContract.Contacts.InterfaceConsts.DisplayName} ASC");

        if (phoneCursor?.MoveToFirst() == true)
        {
            do
            {
                try
                {
                    raw.Add(new RawDeviceContact
                    {
                        Id = phoneCursor.GetString(0),
                        DisplayName = phoneCursor.GetString(1),
                        PhoneNumber = phoneCursor.GetString(2)
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"������ ������ ��������: {ex.Message}");
                }
            }
            while (phoneCursor.MoveToNext());
        }

        phoneCursor?.Close();

        var emailProjection = new[]
        {
            ContactsContract.Contacts.InterfaceConsts.Id,
            ContactsContract.CommonDataKinds.Email.Address
        };

        var emailCursor = _context.ContentResolver?.Query(
            ContactsContract.CommonDataKinds.Email.ContentUri,
            emailProjection,
            null,
            null,
            null);

        if (emailCursor?.MoveToFirst() == true)
        {
            do
            {
                try
                {
                    raw.Add(new RawDeviceContact
                    {
                        Id = emailCursor.GetString(0),
                        Email = emailCursor.GetString(1)
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"������ ������ email: {ex.Message}");
                }
            }
            while (emailCursor.MoveToNext());
        }

        emailCursor?.Close();
        return raw;
    }
#endif
}
