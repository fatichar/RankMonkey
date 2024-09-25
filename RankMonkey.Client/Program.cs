using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RankMonkey.Client;
using RankMonkey.Client.Auth;
using RankMonkey.Client.Services;
using Serilog;
using Serilog.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.BrowserConsole()
    .CreateLogger();

// Use Serilog for logging
builder.Logging.AddProvider(new SerilogLoggerProvider(Log.Logger));

// Configure JSON serialization options
builder.Services.AddScoped<JsonSerializerOptions>(_ => new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters = { new JsonStringEnumConverter() }
});

builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();

// Add this line to load the configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add HTTP message handler for authorization
builder.Services.AddScoped<AuthorizationMessageHandler>();

// Use the current base address for the authorized client
var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);

// Register the authorized HttpClient
builder.Services.AddHttpClient("AuthorizedClient", client =>
{
    client.BaseAddress = baseAddress;
})
.AddHttpMessageHandler<AuthorizationMessageHandler>();

// Register a named HttpClient for the server API
var serverUrl = builder.Configuration["ServerUrl"];
if (!string.IsNullOrEmpty(serverUrl))
{
    builder.Services.AddHttpClient("ServerAPI", client =>
    {
        client.BaseAddress = new Uri(serverUrl);
    })
    .AddHttpMessageHandler<AuthorizationMessageHandler>();
}
else
{
    throw new Exception("ServerUrl not found in configuration");
}

// Register factory methods for HttpClient
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthorizedClient"));
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

var host = builder.Build();

await host.RunAsync();

Log.CloseAndFlush();