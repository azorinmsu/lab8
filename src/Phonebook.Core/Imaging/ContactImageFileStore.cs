namespace Phonebook.Imaging;

public sealed class ContactImageFileStore
{
    public ContactImageFileStore(string directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
            throw new ArgumentException("Каталог изображений не задан.", nameof(directory));

        DirectoryPath = directory;
        Directory.CreateDirectory(directory);
    }

    public string DirectoryPath { get; }

    public async Task<string> SaveAsync(string fileName, byte[] content, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(content);

        var path = Path.Combine(DirectoryPath, fileName);
        await File.WriteAllBytesAsync(path, content, cancellationToken).ConfigureAwait(false);
        return path;
    }

    public bool Exists(string fileName) => File.Exists(Path.Combine(DirectoryPath, fileName));

    public void Delete(string fileName)
    {
        var path = Path.Combine(DirectoryPath, fileName);
        if (File.Exists(path))
            File.Delete(path);
    }
}
