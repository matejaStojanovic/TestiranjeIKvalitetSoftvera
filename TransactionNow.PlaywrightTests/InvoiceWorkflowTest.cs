using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Microsoft.Playwright.Assertions;

namespace TransactionNow.PlaywrightTests;

public class InvoiceWorkflowTest : BaseTest
{
    private async Task<int> GetBalance()
    {
        var heading = _page.GetByRole(AriaRole.Heading)
                           .Filter(new() { HasText = "Balance:" });

        var text = await heading.InnerTextAsync();
        var match = Regex.Match(text, @"\d+");

        return int.Parse(match.Value);
    }

    [Test]
    public async Task Full_Invoice_Flow_EndToEnd()
    {
        int invoice1 = 200;
        int invoice2 = 400;
        long hugeInvoice = 999999999999999999;


        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })
                   .FillAsync("admin@test.com");

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })
                   .FillAsync("Test123!");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" })
                   .ClickAsync();

        var adminStartBalance = await GetBalance();

        await _page.GetByRole(AriaRole.Link, new() { Name = "Invoices" })
                   .ClickAsync();


        await _page.GetByPlaceholder("Recipient Email")
                   .FillAsync("user1@test.com");

        await _page
            .Locator("form")
            .Filter(new() { HasText = "Send Invoice" })
            .GetByPlaceholder("Amount")
            .FillAsync(invoice1.ToString());

        await _page.GetByRole(AriaRole.Button, new() { Name = "Send Invoice" })
                   .ClickAsync();

        await _page.GetByPlaceholder("Recipient Email")
                   .FillAsync("user1@test.com");

        await _page
            .Locator("form")
            .Filter(new() { HasText = "Send Invoice" })
            .GetByPlaceholder("Amount")
            .FillAsync(invoice2.ToString());

        await _page.GetByRole(AriaRole.Button, new() { Name = "Send Invoice" })
                   .ClickAsync();

        await _page.GetByPlaceholder("Recipient Email")
                   .FillAsync("user1@test.com");

        await _page
            .Locator("form")
            .Filter(new() { HasText = "Send Invoice" })
            .GetByPlaceholder("Amount")
            .FillAsync(hugeInvoice.ToString());

        await _page.GetByRole(AriaRole.Button, new() { Name = "Send Invoice" })
                   .ClickAsync();

        await _page.GetByPlaceholder("Recipient Email")
                   .FillAsync("wronguser@gmail.com");

        await _page
            .Locator("form")
            .Filter(new() { HasText = "Send Invoice" })
            .GetByPlaceholder("Amount")
            .FillAsync("100");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Send Invoice" })
                   .ClickAsync();

        await Expect(_page.GetByText("Receiver not found"))
            .ToBeVisibleAsync();


        await _page.GetByRole(AriaRole.Link, new() { Name = "Dashboard" })
                   .ClickAsync();

        await _page.GetByRole(AriaRole.Button, new() { Name = "Logout" })
                   .ClickAsync();


        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })
                   .FillAsync("user1@test.com");

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })
                   .FillAsync("Test123!");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" })
                   .ClickAsync();

        var userStartBalance = await GetBalance();


        await _page.GetByRole(AriaRole.Link, new() { Name = "Cards" })
                   .ClickAsync();

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Card number" })
                   .FillAsync("99998888");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Add Card" })
                   .ClickAsync();


        await _page.GetByRole(AriaRole.Link, new() { Name = "Transactions" })
                   .ClickAsync();

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Card Number" })
                   .FillAsync("99998888");

        await _page
            .Locator("form")
            .Filter(new() { HasText = "Deposit" })
            .GetByPlaceholder("Amount")
            .FillAsync("1000");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Deposit" })
                   .ClickAsync();


        await _page.GetByRole(AriaRole.Link, new() { Name = "Dashboard" })
                   .ClickAsync();

        var userAfterDeposit = await GetBalance();


        await _page.GetByRole(AriaRole.Link, new() { Name = "Invoices" })
                   .ClickAsync();

        await _page.GetByRole(AriaRole.Button, new() { Name = "Pay" })
                   .First
                   .ClickAsync();

        await _page.GetByRole(AriaRole.Button, new() { Name = "Pay" })
                   .First
                   .ClickAsync();


        await _page.GetByRole(AriaRole.Button, new() { Name = "Pay" })
                   .First
                   .ClickAsync();

        await Expect(_page.GetByText("Insufficient funds."))
            .ToBeVisibleAsync();


        await _page.GetByRole(AriaRole.Button, new() { Name = "Delete" })
                   .First
                   .ClickAsync();


        await _page.GetByRole(AriaRole.Link, new() { Name = "Dashboard" })
                   .ClickAsync();

        var userFinalBalance = await GetBalance();

        var expectedUser = userAfterDeposit - invoice1 - invoice2;

        Assert.That(userFinalBalance, Is.EqualTo(expectedUser));

        await _page.GetByRole(AriaRole.Button, new() { Name = "Logout" })
                   .ClickAsync();

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })
                   .FillAsync("admin@test.com");

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })
                   .FillAsync("Test123!");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" })
                   .ClickAsync();

        var adminFinalBalance = await GetBalance();

        var expectedAdmin = adminStartBalance + invoice1 + invoice2;

        Assert.That(adminFinalBalance, Is.EqualTo(expectedAdmin));
    }
}
