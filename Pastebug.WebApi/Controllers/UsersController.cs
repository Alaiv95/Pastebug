using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pastebug.BLL.Dtos;
using Pastebug.BLL.Services;

namespace Pastebug.WebApi.Controllers;

[ApiController]
public class UsersController : ControllerBase
{
    private IUserService _usersService;

    public UsersController(IUserService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost("api/[controller]/Register")]
    [Produces("text/plain")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
        Guid result = await _usersService.AddUserAsync(userDto);

        return result == Guid.Empty ? BadRequest() : Ok(result.ToString());
    }

    [HttpPost("api/[controller]/SignIn")]
    [Produces("text/plain")]
    public async Task<IActionResult> SignIn([FromBody] UserDto userDto)
    {
        string result = await _usersService.SignInAsync(userDto);

        return Ok(result);
    }
}
