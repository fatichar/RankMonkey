using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using RankMonkey.Shared.Models;

namespace RankMonkey.Client.Pages;

public partial class Ranking
{
    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = default!;

    private HttpClient Http => HttpClientFactory.CreateClient("ServerAPI");

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
            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error fetching metrics" + ex.Message;
        }
    }

    private async Task SubmitMetrics()
    {
        var metrics = new UpdateMetricsRequest(model.Income, model.NetWorth, model.Currency);
        try
        {
            await Http.PutAsJsonAsync("api/metrics", metrics);
            await FetchRanking();
            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error submitting metrics");
            ErrorMessage = "Error submitting metrics" + ex.Message;
        }
    }

    public string? ErrorMessage { get; set; }

    private async Task FetchRanking()
    {
        try
        {
            ranking = await Http.GetFromJsonAsync<RankingDto>("api/ranking");
            ErrorMessage = null;
            _ = InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching ranking");
            ErrorMessage = "Error fetching ranking" + ex.Message;
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