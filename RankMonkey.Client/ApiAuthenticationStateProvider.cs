// using System.Net.Http.Json;
// using System.Security.Claims;
// using Microsoft.AspNetCore.Components.Authorization;
//
// namespace RankMonkey.Client;
//
// public class ApiAuthenticationStateProvider : AuthenticationStateProvider
// {
//     private readonly HttpClient _httpClient;
//
//     public ApiAuthenticationStateProvider(HttpClient httpClient)
//     {
//             _httpClient = httpClient;
//         }
//
//     public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//     {
//             try
//             {
//                 var response = await _httpClient.GetFromJsonAsync<UserInfo>("api/auth/user");
//
//                 if (response?.IsAuthenticated ?? false)
//                 {
//                     var claims = response.Claims.Select(c => new Claim(c.Type, c.Value));
//                     var identity = new ClaimsIdentity(claims, "Google");
//                     var user = new ClaimsPrincipal(identity);
//                     return new AuthenticationState(user);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Error in GetAuthenticationStateAsync: {ex.Message}");
//             }
//
//             return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
//         }
// }
//
// public class UserInfo
// {
//     public bool IsAuthenticated { get; set; }
//     public IEnumerable<ClaimInfo> Claims { get; set; }
// }
//
// public class ClaimInfo
// {
//     public string Type { get; set; }
//     public string Value { get; set; }
// }