namespace Phonebook.Platform;

public enum ContactsAccess
{
    Granted,
    Denied,
    Revoked
}

/// <summary>
/// Решения приложения по READ_CONTACTS.
/// Одноразовое разрешение «только в этот раз» для READ_CONTACTS платформа не даёт:
/// оно относится к разрешениям вроде геолокации, камеры и микрофона.
/// До API 23 опасное разрешение выдавалось при установке.
/// </summary>
public static class ContactsPermissionPolicy
{
    public const string PermissionName = "android.permission.READ_CONTACTS";

    public const int RuntimePermissionApiLevel = 23;

    public const bool SupportsOneTimeGrant = false;

    public static bool ShouldReadContacts(ContactsAccess access) => access == ContactsAccess.Granted;

    public static bool IsGrantedAtInstall(int apiLevel) => apiLevel > 0 && apiLevel < RuntimePermissionApiLevel;

    public static bool CanRequestAgain(ContactsAccess access) => access is ContactsAccess.Denied or ContactsAccess.Revoked;
}
