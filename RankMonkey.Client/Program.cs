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
builder.Services.AddHttpClient("AuthorizedClient",
        client => client.BaseAddress = new Uri(builder.Configuration["ServerUrl"] ??
                                               throw new Exception("ServerUrl not found in configuration")))
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthorizedClient"));

await builder.Build().RunAsync();

Log.CloseAndFlush();