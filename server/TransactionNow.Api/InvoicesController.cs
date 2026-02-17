using Microsoft.AspNetCore.Mvc;
using TransactionNow.Application.Services;
using TransactionNow.Api.Helpers;
using TransactionNow.Domain.Entities;
using TransactionNow.Infrastructure.Repositories;

namespace TransactionNow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly InvoiceService _invoiceService;

    public InvoicesController(InvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDTO dto)
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        if (userId == null)
            return Unauthorized();

        try
        {
            await _invoiceService.SendInvoice(userId, dto.ReceiverEmail, dto.Amount);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMyInvoices()
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        if (userId == null)
            return Unauthorized();

        var invoices = await _invoiceService.GetInvoicesForUser(userId);

        return Ok(invoices);
    }

    [HttpPost("{id}/pay")]
    public async Task<IActionResult> PayInvoice(string id)
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        if (userId == null)
            return Unauthorized();

        try
        {
            await _invoiceService.PayInvoice(userId, id);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvoice(string id)
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        if (userId == null)
            return Unauthorized();

        try
        {
            await _invoiceService.DeleteInvoice(userId, id);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
