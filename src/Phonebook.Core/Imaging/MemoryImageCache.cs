namespace Phonebook.Imaging;

/// <summary>
/// Память кэша изображений с теми же правилами, что исходный статический словарь:
/// повторная запись не заменяет значение, отсутствующий ключ возвращает null.
/// ETag и Last-Modified не поддерживаются.
/// </summary>
public sealed class MemoryImageCache
{
    private readonly Dictionary<string, string> _cache = new();

    public int Count => _cache.Count;

    public const bool SupportsValidators = false;

    public string? TryGet(string? url)
    {
        if (url == null)
            return null;

        return _cache.TryGetValue(url, out var value) ? value : null;
    }

    public void Add(string url, string localPath)
    {
        ArgumentNullException.ThrowIfNull(url);
        if (!_cache.ContainsKey(url))
            _cache[url] = localPath;
    }

    public void Clear() => _cache.Clear();

    public bool Invalidate(string url) => url != null && _cache.Remove(url);
}
