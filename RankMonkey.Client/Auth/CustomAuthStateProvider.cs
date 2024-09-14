using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using RankMonkey.Client.Services;

namespace RankMonkey.Client.Auth;

public class CustomAuthStateProvider(HttpClient httpClient, ILocalStorageService storage)
        : AuthenticationStateProvider
{
    private const string AUTH_TOKEN = "authToken";
    private readonly AuthenticationState _anonymous = new (new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await storage.GetItemAsync<string>(AUTH_TOKEN);

        return CreateAuthState(token);
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await storage.SetItemAsync(AUTH_TOKEN, token);

        SetAuthorization(token);

        var authState = CreateAuthState(token);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    private AuthenticationState CreateAuthState(string token)
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

    private void ClearAuthenticationState()
    {
        httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task MarkUserAsLoggedOut()
    {
        await storage.RemoveItemAsync(AUTH_TOKEN);

        ClearAuthenticationState();

        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }
}