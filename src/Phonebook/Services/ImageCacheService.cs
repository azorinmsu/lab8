using Phonebook.Imaging;

namespace Phonebook.Services;

public static class ImageCacheService
{
    private static readonly MemoryImageCache Cache = new();

    public static string? GetCachedImage(string url) => Cache.TryGet(url);

    public static void CacheImage(string url, string localPath) => Cache.Add(url, localPath);

    public static void ClearCache() => Cache.Clear();

    public static bool Invalidate(string url) => Cache.Invalidate(url);
}
