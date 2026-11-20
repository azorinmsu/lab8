using SQLite;

namespace Phonebook.Data;

/// <summary>
/// Где приложение хранит данные. Флаги SQLite не задают режим доступа Android.
/// </summary>
public static class LocalDataPlacement
{
    public const string DatabaseFileName = "contacts.db3";

    public const string ImageFolderName = "ContactImages";

    public const bool AndroidAllowBackup = true;

    public static SQLiteOpenFlags OpenFlags { get; } =
        SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache;

    public const string IsolationSummary =
        "Файл базы создаётся в каталоге приложения (FileSystem.AppDataDirectory). " +
        "На Android это внутреннее хранилище пакета, недоступное другим UID. " +
        "SQLiteOpenFlags.SharedCache включает разделяемый кэш SQLite в процессе, а не доступ другим приложениям. " +
        "android:allowBackup=true разрешает включать файл в резервную копию.";
}
