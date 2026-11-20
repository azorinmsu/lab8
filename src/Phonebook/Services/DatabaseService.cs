using Phonebook.Configuration;
using Phonebook.Data;

namespace Phonebook.Services;

public class DatabaseService : ContactRepository
{
    public DatabaseService()
        : base(ResolvePath(resetTestingDatabase: true))
    {
    }

    public DatabaseService(string databasePath)
        : base(databasePath)
    {
    }

    public static string ResolvePath(bool resetTestingDatabase)
    {
        var testing = PhonebookEnvironmentSelector.Current == PhonebookEnvironment.Testing;
        var root = testing
            ? Path.Combine(FileSystem.CacheDirectory, "phonebook-testing")
            : FileSystem.AppDataDirectory;

        Directory.CreateDirectory(root);
        var path = Path.Combine(root, LocalDataPlacement.DatabaseFileName);
        if (testing && resetTestingDatabase && File.Exists(path))
            File.Delete(path);

        return path;
    }
}
