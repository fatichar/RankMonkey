using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RankMonkey.Shared.Models;
using System.Net.Http.Json;

namespace RankMonkey.Client.Pages;
public partial class Index
{
    private string GoogleClientId => Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Google Client ID not found in configuration.");
    private const string GOOGLE_LOGIN_API = "api/auth/login/google";
    private const string GUEST_LOGIN_API = "api/auth/login/guest";
    private const string LOGOUT_API = "api/auth/logout";
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

    private static Index? _instance;

    protected override async Task OnInitializedAsync()
    {
        _instance = this;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitializeGoogleSignIn();
        }
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
            var response = await Http.PostAsJsonAsync(GOOGLE_LOGIN_API, request);
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

                NavigationManager.NavigateTo("/", true);
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
            var response = await Http.PostAsync(GUEST_LOGIN_API, null);
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
            var response = await Http.PostAsync(LOGOUT_API, null);
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
            NavigationManager.NavigateTo("/", true);
        }
    }
}