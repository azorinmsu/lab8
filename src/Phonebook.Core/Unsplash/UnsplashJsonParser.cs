using System.Text.Json;
using Phonebook.Models;

namespace Phonebook.Unsplash;

public static class UnsplashJsonParser
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static UnsplashSearchResult ParseSearch(string json)
    {
        var result = JsonSerializer.Deserialize<UnsplashSearchResult>(json, Options);
        return result ?? new UnsplashSearchResult();
    }

    public static List<UnsplashImage> ParseRandom(string json)
    {
        var images = JsonSerializer.Deserialize<List<UnsplashImage>>(json, Options) ?? [];
        foreach (var image in images)
            image.Urls ??= new UnsplashUrls();

        return images;
    }
}
