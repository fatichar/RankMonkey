using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using RankMonkey.Shared.Models;

namespace RankMonkey.Client.Pages;

public partial class Ranking
{
    private class Model
    {
        [Required] public string Currency { get; set; } = "INR";

        [Required]
        [Range(0, long.MaxValue, ErrorMessage = "Income must be a positive number.")]
        public long Income { get; set; }

        [Required]
        [Range(long.MinValue, long.MaxValue, ErrorMessage = "Net Worth must be a valid number.")]
        public long NetWorth { get; set; }
    }

    private Model model = new();
    private RankingDto? ranking = null;
    private string userName = string.Empty;

    protected override async void OnInitialized()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        userName = authState.User.Identity?.Name ?? string.Empty;
        userName = userName.Split(" ")[0];
        // notify property changed to trigger UI update
        _ = InvokeAsync(StateHasChanged);

        await FetchMetrics();
    }

    private async Task FetchMetrics()
    {
        try
        {
            var metrics = await Http.GetFromJsonAsync<MetricsDto>("api/metrics");
            model.Income = metrics.Income;
            model.NetWorth = metrics.NetWorth;
            model.Currency = metrics.Currency;
            await FetchRanking();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching metrics");
            // TODO: Display error message to user
        }
    }

    private async Task SubmitMetrics()
    {
        var metrics = new UpdateMetricsRequest(model.Income, model.NetWorth, model.Currency);
        try
        {
            await Http.PutAsJsonAsync("api/metrics", metrics);
            await FetchRanking();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error submitting metrics");
            // TODO: Display error message to user
        }
    }

    private async Task FetchRanking()
    {
        try
        {
            ranking = await Http.GetFromJsonAsync<RankingDto>("api/ranking");
            _ = InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching ranking");
            // TODO: Display error message to user
        }
    }

    private string FormatPercentile(float percentile)
    {
        return percentile < 0 ? "-" : $"{percentile:P2}";
    }

    private string GetCurrencySymbol()
    {
        return model.Currency == "USD" ? "$" : "₹";
    }
}