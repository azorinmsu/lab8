using Phonebook.Models;

namespace Phonebook.Unsplash;

public interface IUnsplashClient
{
    Task<List<UnsplashImage>> SearchImagesAsync(string query, int page = 1, int perPage = 12, CancellationToken cancellationToken = default);

    Task<List<UnsplashImage>> GetRandomImagesAsync(int count = 12, CancellationToken cancellationToken = default);

    Task<string?> DownloadImageAsync(string imageUrl, string fileName, CancellationToken cancellationToken = default);
}
