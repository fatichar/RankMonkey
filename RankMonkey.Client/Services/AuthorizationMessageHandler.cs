using System.Net.Http.Headers;

namespace RankMonkey.Client.Services;

public class AuthorizationMessageHandler(ILocalStorageService localStorage) : DelegatingHandler
{
    private readonly string[] _anonymousUrls = new[]
    {
        "api/auth/login",
        "api/auth/register",
        "api/public"
        // Add any other URLs that should not include the JWT token
    };

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await AddAuthToken(localStorage, request);

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task AddAuthToken(ILocalStorageService localStorage, HttpRequestMessage request)
    {
        if (ShouldAddToken(request.RequestUri))
        {
            var token = await localStorage.TryGetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    private bool ShouldAddToken(Uri? requestUri)
    {
        if (requestUri == null) return false;
        return !_anonymousUrls.Any(url => requestUri.AbsolutePath.Contains(url, StringComparison.OrdinalIgnoreCase));
    }
}