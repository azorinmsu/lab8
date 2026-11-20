using Phonebook.Models;

namespace Phonebook.Unsplash;

/// <summary>
/// Локальные изображения для окружения Testing. Сеть не используется.
/// </summary>
public sealed class FakeUnsplashClient : IUnsplashClient
{
    public Task<List<UnsplashImage>> SearchImagesAsync(string query, int page = 1, int perPage = 12, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Sample(string.IsNullOrWhiteSpace(query) ? "sample" : query.Trim()));
    }

    public Task<List<UnsplashImage>> GetRandomImagesAsync(int count = 12, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Sample("random").Take(Math.Max(0, count)).ToList());
    }

    public Task<string?> DownloadImageAsync(string imageUrl, string fileName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<string?>(Path.Combine("testing-images", fileName));
    }

    private static List<UnsplashImage> Sample(string idPrefix) =>
    [
        new UnsplashImage
        {
            Id = idPrefix + "-1",
            Description = "Учебное изображение",
            AltDescription = "Фиктивный ответ Unsplash",
            Urls = new UnsplashUrls
            {
                Thumb = "https://example.test/thumb.jpg",
                Regular = "https://example.test/regular.jpg",
                Small = "https://example.test/small.jpg"
            }
        }
    ];
}
