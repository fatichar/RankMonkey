using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using HowRich.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => 
    new HttpClient
    {
        BaseAddress = new Uri("https://localhost:5001/")
    });

builder.Services.AddOidcAuthentication(options =>
{
    //builder.Configuration.Bind("Authentication", options.ProviderOptions);
    options.ProviderOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ProviderOptions.Authority = "https://accounts.google.com";
    options.ProviderOptions.ResponseType = "id_token token";
});

builder.Services.AddAuthorizationCore();
//builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

await builder.Build().RunAsync();