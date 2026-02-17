using Microsoft.AspNetCore.Mvc;
using TransactionNow.Application.Interfaces;
using TransactionNow.Api.Helpers;

namespace TransactionNow.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UserController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        if (userId == null)
            return Unauthorized();

        var user = await _repository.GetById(userId);
        if (user == null)
            return NotFound();

        return Ok(new
        {
            user.Id,
            user.Ime,
            user.Prezime,
            user.Email,
            user.Balance
        });
    }
}
