using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using RankMonkey.Client.Services;

namespace RankMonkey.Client.Auth;

public class CustomAuthStateProvider(HttpClient httpClient, ILocalStorageService storage)
    : AuthenticationStateProvider
{
    private const string AUTH_TOKEN = "authToken";
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await storage.TryGetItemAsync<string>(AUTH_TOKEN);

        return CreateAuthState(token);
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await storage.SetItemAsync(AUTH_TOKEN, token);

        SetAuthorization(token);

        var authState = CreateAuthState(token);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await storage.RemoveItemAsync(AUTH_TOKEN);

        ClearAuthorizationState();

        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
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

    private void SetAuthorization(string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
    }

    private void ClearAuthorizationState()
    {
        httpClient.DefaultRequestHeaders.Authorization = null;
    }
}