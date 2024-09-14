using Microsoft.JSInterop;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RankMonkey.Client.Services;

public class LocalStorageService(IJSRuntime jsRuntime, ILogger<LocalStorageService> logger) : ILocalStorageService
{
    private const string LOCALSTORAGE_GETITEM = "localStorage.getItem";

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<T> GetItemAsync<T>(string key)
    {
        var json = await jsRuntime.InvokeAsync<string?>(LOCALSTORAGE_GETITEM, key);

        if (json == null)
        {
            throw new InvalidOperationException("Key not found in local storage");
        }

        return JsonSerializer.Deserialize<T>(json, _jsonOptions) ??
               throw new InvalidDataException($"Failed to deserialize json: {json} for key: {key}");
    }

    public async Task<T?> TryGetItemAsync<T>(string key)
    {
        try
        {
            return await GetItemAsync<T>(key);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get item from local storage");
        };
        return default;
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value, _jsonOptions));
    }

    public async Task RemoveItemAsync(string key)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}