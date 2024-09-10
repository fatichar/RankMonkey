using Microsoft.JSInterop;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RankMonkey.Client.Services;

public class LocalStorageService(IJSRuntime jsRuntime, ILogger<LocalStorageService> logger) : ILocalStorageService
{
    private const string LOCALSTORAGE_GET_ITEM = "localStorage.getItem";

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<T> GetItemAsync<T>(string key)
    {
        var item = await TryGetItemAsync<T>(key);

        if (item == null)
        {
            throw new KeyNotFoundException($"Key {key} not found in local storage");
        }

        return item;
    }

    public async Task<T?> TryGetItemAsync<T>(string key)
    {
        var json = await jsRuntime.InvokeAsync<string?>(LOCALSTORAGE_GET_ITEM, key);

        try
        {
            return json == null ? default : JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (JsonException e)
        {
            logger.LogError(e, "Failed to deserialize json: {json} for key: {key}", json, key);
            throw new InvalidDataException($"Failed to deserialize json: {json} for key: {key}");
        }
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        logger.LogInformation("Setting item {key} to {value}", key, value);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value, _jsonOptions));
    }

    public async Task RemoveItemAsync(string key)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}