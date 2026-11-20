using System.Text.Json.Serialization;

namespace Phonebook.Models;

public class UnsplashImage
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("urls")]
    public UnsplashUrls Urls { get; set; } = new();

    [JsonPropertyName("alt_description")]
    public string? AltDescription { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; } = string.Empty;
}

public class UnsplashUrls
{
    [JsonPropertyName("raw")]
    public string Raw { get; set; } = string.Empty;

    [JsonPropertyName("full")]
    public string Full { get; set; } = string.Empty;

    [JsonPropertyName("regular")]
    public string Regular { get; set; } = string.Empty;

    [JsonPropertyName("small")]
    public string Small { get; set; } = string.Empty;

    [JsonPropertyName("thumb")]
    public string Thumb { get; set; } = string.Empty;
}

public class UnsplashSearchResult
{
    public List<UnsplashImage> Results { get; set; } = new();

    public int Total { get; set; }

    public int TotalPages { get; set; }
}
