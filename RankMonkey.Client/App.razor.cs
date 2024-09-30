using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace RankMonkey.Client;

public partial class App
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected override void OnInitialized()
    {
        NavigationManager.RegisterLocationChangingHandler(OnLocationChanging);
    }

    private ValueTask OnLocationChanging(LocationChangingContext context)
    {
        // Add any custom logic for location changes here
        return ValueTask.CompletedTask;
    }
}