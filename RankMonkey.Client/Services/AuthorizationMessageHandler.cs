using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace RankMonkey.Client.Services;

public class AuthorizationMessageHandler(ILocalStorageService localStorage, ILogger<AuthorizationMessageHandler> logger) : DelegatingHandler
{
    private const string AUTH_TOKEN_KEY = "authToken";
    private readonly string[] _anonymousUrls =
    [
        "api/auth/login",
        "api/auth/register"
    ];

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await AddAuthToken(localStorage, request);

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task AddAuthToken(ILocalStorageService localStorage, HttpRequestMessage request)
    {
        if (ShouldAddToken(request.RequestUri))
        {
            logger.LogInformation("Attempting to retrieve auth token for request to {RequestUri}", request.RequestUri);
            var token = await localStorage.TryGetItemAsync<string>(AUTH_TOKEN_KEY);
            if (!string.IsNullOrEmpty(token))
            {
                logger.LogInformation("Auth token found. Adding to request headers.");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                logger.LogWarning("Auth token not found in local storage for request to {RequestUri}", request.RequestUri);
            }
        }
    }

    private bool ShouldAddToken(Uri? requestUri)
    {
        if (requestUri == null) return false;
        return !_anonymousUrls.Any(url => requestUri.AbsolutePath.Contains(url, StringComparison.OrdinalIgnoreCase));
    }
}