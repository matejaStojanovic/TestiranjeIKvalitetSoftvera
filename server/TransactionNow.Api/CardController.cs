using Microsoft.AspNetCore.Mvc;
using TransactionNow.Application.Services;
using TransactionNow.Api.Helpers;
using TransactionNow.Domain.Entities;
using TransactionNow.Infrastructure.Repositories;

namespace TransactionNow.Api.Controllers;

[ApiController]
[Route("api/cards")]
public class CardController : ControllerBase
{
    private readonly CardService _cardService;

    public CardController(CardService cardService)
    {
        _cardService = cardService;
    }

    [HttpPost]
    public async Task<IActionResult> AddCard([FromBody] AddKarticaDTO dto)
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        if (userId == null)
            return Unauthorized();

        var user = await _cardService.GetUserWithCards(userId);
        if (user == null)
            return Unauthorized();
        if (user.Cards.Any(c => c.CardNumber == dto.BrojKartice))
        {
            return Conflict(new { error = "Card with this number already exists." });
        }

        await _cardService.AddCard(userId, dto.BrojKartice);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetMyCards()
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        if (userId == null)
            return Unauthorized();

        var user = await _cardService.GetUserWithCards(userId);
        if (user == null)
            return NotFound();

        return Ok(user.Cards.Select(c => new
        {
            c.Id,
            c.CardNumber,
            c.AddedAt
        }));
    }


    [HttpDelete("{cardId}")]
    public async Task<IActionResult> DeleteCard(string cardId)
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        if (userId == null)
            return Unauthorized();

        await _cardService.RemoveCard(userId, cardId);

        return Ok();
    }
}
