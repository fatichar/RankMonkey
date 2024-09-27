using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using RankMonkey.Client.Auth;

namespace RankMonkey.Client.Services;

public class CustomAuthStateProvider(HttpClient httpClient, ILocalStorageService storage)
    : AuthenticationStateProvider
{
    private const string AUTH_TOKEN = "authToken";
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await storage.TryGetItemAsync<string>(AUTH_TOKEN);
        Console.WriteLine($"GetAuthenticationStateAsync called, token exists: {!string.IsNullOrEmpty(token)}");
        return CreateAuthState(token);
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await storage.SetItemAsync(AUTH_TOKEN, token);

        var authState = CreateAuthState(token);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));

        // Force a refresh of the authentication state
        _ = GetAuthenticationStateAsync();
    }

    public async Task MarkUserAsLoggedOut()
    {
        await storage.RemoveItemAsync(AUTH_TOKEN);
        ClearAuthorizationState();
        Console.WriteLine("User marked as logged out");

        // Immediately update the authentication state
        var authState = await GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    private AuthenticationState CreateAuthState(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return _anonymous;
        }

        var claims = JwtUtil.ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var authenticatedUser = new ClaimsPrincipal(identity);
        return new AuthenticationState(authenticatedUser);
    }

    private void ClearAuthorizationState()
    {
        httpClient.DefaultRequestHeaders.Authorization = null;
    }
}