namespace Phonebook.Unsplash;

public static class UnsplashUrlBuilder
{
    public const string BaseUrl = "https://api.unsplash.com";

    /// <summary>
    /// Поиск не добавляет client_id и orientation. Ключ передаётся заголовком Authorization.
    /// </summary>
    public static string BuildSearchUrl(string query, int page = 1, int perPage = 12)
    {
        var safeQuery = query ?? string.Empty;
        return $"{BaseUrl}/search/photos?query={Uri.EscapeDataString(safeQuery)}&page={page}&per_page={perPage}";
    }

    public static string BuildRandomUrl(int count = 12) =>
        $"{BaseUrl}/photos/random?count={count}&orientation=squarish";
}
