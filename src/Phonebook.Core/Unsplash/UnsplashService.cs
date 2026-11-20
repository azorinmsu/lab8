using Phonebook.Imaging;
using Phonebook.Models;

namespace Phonebook.Unsplash;

/// <summary>
/// Клиент Unsplash. Публичный Access Key читается из аргумента или переменной окружения
/// <see cref="AccessKeyEnvironmentVariable"/> и не пишется в журнал.
/// Ошибка HTTP и некорректный JSON приводят к пустому списку, как в исходном сервисе.
/// Загрузка файла не проверяет ETag и не смотрит в <see cref="MemoryImageCache"/>.
/// </summary>
public sealed class UnsplashService : IUnsplashClient
{
    public const string AccessKeyEnvironmentVariable = "UNSPLASH_ACCESS_KEY";
    public const string ClientHeaderName = "Authorization";

    private readonly HttpClient _httpClient;
    private readonly ContactImageFileStore _fileStore;

    public UnsplashService(HttpClient httpClient, string imagesDirectory, string? accessKey = null, bool readEnvironment = true)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _fileStore = new ContactImageFileStore(imagesDirectory);

        var key = accessKey;
        if (string.IsNullOrWhiteSpace(key) && readEnvironment)
            key = Environment.GetEnvironmentVariable(AccessKeyEnvironmentVariable);

        AccessKeyConfigured = !string.IsNullOrWhiteSpace(key);

        if (!string.IsNullOrWhiteSpace(key) && !_httpClient.DefaultRequestHeaders.Contains(ClientHeaderName))
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(ClientHeaderName, $"Client-ID {key.Trim()}");

        if (!_httpClient.DefaultRequestHeaders.Contains("Accept-Version"))
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Version", "v1");
    }

    public bool AccessKeyConfigured { get; }

    public async Task<List<UnsplashImage>> SearchImagesAsync(
        string query,
        int page = 1,
        int perPage = 12,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = UnsplashUrlBuilder.BuildSearchUrl(query, page, perPage);
            using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return [];

            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return UnsplashJsonParser.ParseSearch(json).Results ?? [];
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error searching images: {ex.Message}");
            return [];
        }
    }

    public async Task<List<UnsplashImage>> GetRandomImagesAsync(int count = 12, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = UnsplashUrlBuilder.BuildRandomUrl(count);
            using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return [];

            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return UnsplashJsonParser.ParseRandom(json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting random images: {ex.Message}");
            return [];
        }
    }

    public async Task<string?> DownloadImageAsync(string imageUrl, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl, cancellationToken).ConfigureAwait(false);
            return await _fileStore.SaveAsync(fileName, imageBytes, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error downloading image: {ex.Message}");
            return null;
        }
    }
}
