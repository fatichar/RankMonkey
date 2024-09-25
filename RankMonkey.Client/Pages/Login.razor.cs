using System.Net;
using Microsoft.JSInterop;
using RankMonkey.Shared.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;

namespace RankMonkey.Client.Pages;

public partial class Login
{
    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = default!;

    private HttpClient Http => HttpClientFactory.CreateClient("ServerAPI");
    

    private string GoogleClientId => Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Google Client ID not found in configuration.");
    private string _errorMessage = string.Empty;
    private string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            StateHasChanged();
        }
    }

    [Parameter]
    [SupplyParameterFromQuery(Name = "returnUrl")]
    public string ReturnUrl { get; set; } = "/";

    private static Login? _instance;

    protected override async Task OnInitializedAsync()
    {
        _instance = this;
        Console.WriteLine("Login OnInitializedAsync called");
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            Console.WriteLine("User is authenticated, redirecting");
            RedirectToReturnUrl();
        }
        else
        {
            Console.WriteLine("User is not authenticated, staying on login page");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitializeGoogleSignIn();
        }
    }

    private void RedirectToReturnUrl()
    {
        var url = string.IsNullOrWhiteSpace(ReturnUrl) ? "/" : Uri.UnescapeDataString(ReturnUrl);
        NavigationManager.NavigateTo(url);
    }

    private async Task InitializeGoogleSignIn()
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("initializeGoogleSignIn", GoogleClientId);
            await JsRuntime.InvokeVoidAsync("renderGoogleSignInButton");
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error initializing Google Sign-In: {ex.Message}";

            Logger.LogError($"Error initializing Google Sign-In: {ex}");
        }
    }

    [JSInvokable]
    public static async Task HandleGoogleLogin(string googleToken)
    {
        if (_instance != null)
        {
            await _instance.LoginUsingGoogle(googleToken);
        }
    }

    private async Task LoginUsingGoogle(string googleToken)
    {
        Logger.LogInformation($"SignIn called with token length: {googleToken.Length}");
        var request = new GoogleLoginRequest(googleToken);
        try
        {
            var response = await Http.PostAsJsonAsync(ApiPaths.GOOGLE_LOGIN_API, request);
            await HandleLoginResponse(response);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
            Logger.LogError(ex, "SignIn error");
        }
    }

    private async Task HandleLoginResponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
            if (token != null)
            {
                Logger.LogInformation("Login successful");

                await AuthStateProvider.MarkUserAsAuthenticated(token.Token);

                RedirectToReturnUrl();
            }
            else
            {
                ErrorMessage = "Failed to deserialize token response";
                Logger.LogError("Failed to deserialize token response");
            }
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            ErrorMessage = $"Login failed: {response.ReasonPhrase}. Error: {errorContent}";
            Logger.LogError("Login failed: {ReasonPhrase}. Error: {ErrorContent}", response.ReasonPhrase, errorContent);
        }
    }

    private async Task LoginAsGuest()
    {
        try
        {
            var response = await Http.PostAsync(ApiPaths.GUEST_LOGIN_API, null);
            await HandleLoginResponse(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task Logout()
    {
        try
        {
            var response = await Http.PostAsync(ApiPaths.LOGOUT_API, null);
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Failed to sign out";
                Logger.LogError("Failed to sign out");
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "SignOut error");
        }
        finally
        {
            await JsRuntime.InvokeVoidAsync("signOut");
            await AuthStateProvider.MarkUserAsLoggedOut();
            NavigationManager.NavigateTo("/login", true);
        }
    }

    private async Task OnSuccessfulLogin()
    {
        Console.WriteLine("Login successful, redirecting");
        await AuthStateProvider.GetAuthenticationStateAsync();
        RedirectToReturnUrl();
    }
}