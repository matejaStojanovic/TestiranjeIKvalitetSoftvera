using Microsoft.AspNetCore.Mvc;
using TransactionNow.Application.Services;
using TransactionNow.Api.Helpers;
using TransactionNow.Domain.Entities;

namespace TransactionNow.Api.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly TransactionService _transactionService;

    public TransactionController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }


    [HttpPost]
    public async Task<IActionResult> Send([FromBody] NovaTransakcijaDTO dto)
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        if (userId == null)
            return Unauthorized();

        await _transactionService.SendMoney(
            userId,
            dto.RecipientEmail,
            dto.Iznos
        );

        return Ok();
    }


    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositDTO dto)
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        if (userId == null)
            return Unauthorized();

        await _transactionService.Deposit(
            userId,
            dto.CardNumber,
            dto.Amount
        );

        return Ok();
    }


    [HttpGet]
    public async Task<IActionResult> GetMyTransactions()
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        if (userId == null)
            return Unauthorized();

        var transactions = await _transactionService.GetUserTransactions(userId);

        return Ok(transactions.Select(t => new
        {
            t.Id,
            t.FromUserId,
            t.ToUserId,
            t.Amount,
            t.DateTime,
            t.Status
        }));
    }
}
