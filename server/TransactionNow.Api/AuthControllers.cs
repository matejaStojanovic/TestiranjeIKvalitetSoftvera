using Microsoft.AspNetCore.Mvc;
using TransactionNow.Application.Services;
using TransactionNow.Domain.Entities;

namespace TransactionNow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        var user = await _userService.Register(
            dto.Ime,
            dto.Prezime,
            dto.Email,
            dto.Password
        );

        return Ok(new
        {
            user.Id,
            user.Ime,
            user.Prezime,
            user.Email,
            user.Balance
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var user = await _userService.Login(dto.Email, dto.Password);

        if (user == null)
            return Unauthorized("Invalid credentials");

        HttpContext.Session.SetString("UserId", user.Id);

        return Ok(new
        {
            user.Id,
            user.Ime,
            user.Prezime,
            user.Email,
            user.Balance
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok();
    }
}
