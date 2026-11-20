namespace Phonebook.TestSupport;

public static class RepoPaths
{
    public static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Phonebook.sln")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Корень репозитория не найден: нет Phonebook.sln.");
    }

    public static string SourceFile(params string[] relativeParts) =>
        Path.Combine(new[] { Root() }.Concat(relativeParts).ToArray());
}
