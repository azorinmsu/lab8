namespace Phonebook.Contacts;

/// <summary>
/// Что именно читает Android-импортёр. Почтовый адрес в проекцию не входит.
/// Имена колонок соответствуют ContactsContract: _id, display_name, data1.
/// </summary>
public static class AndroidContactsContractMap
{
    public static readonly string[] PhoneColumns = ["_id", "display_name", "data1"];

    public static readonly string[] EmailColumns = ["_id", "data1"];

    public const bool IncludesPostalAddress = false;

    public const string ReadContactsPermission = "android.permission.READ_CONTACTS";
}
