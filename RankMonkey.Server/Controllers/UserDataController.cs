using RankMonkey.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RankMonkey.Server.Services;

namespace RankMonkey.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserDataController : ControllerBase
{
    private readonly IUserDataService _userDataService;

    public UserDataController(IUserDataService userDataService)
    {
        _userDataService = userDataService;
    }

    // [HttpPost]
    // public async Task<ActionResult<Ranking>> SubmitUserData(UserInfo userInfo)
    // {
    //     var userId = User.FindFirst("sub")?.Value;
    //     if (string.IsNullOrEmpty(userId))
    //     {
    //         return Unauthorized();
    //     }
    //
    //     try
    //     {
    //         userInfo.Id = int.Parse(userId);
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         throw;
    //     }
    //     var ranking = await _userDataService.SubmitUserDataAsync(userInfo);
    //     return Ok(ranking);
    // }

    [HttpGet]
    public async Task<ActionResult<UserInfo>> GetUserData()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var userData = await _userDataService.GetUserDataAsync(userId);
        return userData != null ? Ok(userData) : NotFound();
    }
}