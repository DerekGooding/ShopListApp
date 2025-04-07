using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopListApp.Commands.Delete;
using ShopListApp.Commands.Update;
using ShopListApp.Interfaces.IServices;
using System.Security.Claims;

namespace ShopListApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody]UpdateUserCommand cmd)
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _userService.UpdateUser(id, cmd);
        return Ok(id);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser([FromBody]DeleteUserCommand cmd)
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _userService.DeleteUser(id, cmd);
        return Ok(id);
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetUser()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userView = await _userService.GetUserById(id);
        return Ok(userView);
    }
}
