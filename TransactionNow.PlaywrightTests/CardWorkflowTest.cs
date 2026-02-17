using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Microsoft.Playwright.Assertions;

namespace TransactionNow.PlaywrightTests;

public class CardWorkflowTests : BaseTest
{
    [Test]
    public async Task Full_Card_Workflow_Should_Work_Correctly()
    {
        string cardNumber = "12341234";


        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })
                   .FillAsync("admin@test.com");

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })
                   .FillAsync("Test123!");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" })
                   .ClickAsync();

        await Expect(_page).ToHaveURLAsync(new Regex(".*dashboard"));


        await _page.GetByRole(AriaRole.Link, new() { Name = "Cards" })
                   .ClickAsync();

        await Expect(_page).ToHaveURLAsync(new Regex(".*cards"));


        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Card number" })
                   .FillAsync(cardNumber);

        await _page.GetByRole(AriaRole.Button, new() { Name = "Add Card" })
                   .ClickAsync();

        await Expect(_page.GetByText($"Card: {cardNumber}"))
            .ToBeVisibleAsync();


        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Card number" })
                   .FillAsync(cardNumber);

        await _page.GetByRole(AriaRole.Button, new() { Name = "Add Card" })
                   .ClickAsync();

        await Expect(
    _page.GetByText("Card with this number already exists.")
).ToBeVisibleAsync();


        var cardBlock = _page.Locator(".card")
                             .Filter(new() { HasText = cardNumber });

        await cardBlock.GetByRole(AriaRole.Button, new() { Name = "Delete" })
                       .ClickAsync();


        await Expect(cardBlock).ToHaveCountAsync(0);



        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Card number" })
                   .FillAsync(cardNumber);

        await _page.GetByRole(AriaRole.Button, new() { Name = "Add Card" })
                   .ClickAsync();

        await Expect(_page.GetByText($"Card: {cardNumber}"))
            .ToBeVisibleAsync();


        await _page.GetByRole(AriaRole.Link, new() { Name = "Transactions" })
                   .ClickAsync();

        await Expect(_page).ToHaveURLAsync(new Regex(".*transactions"));

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Card Number" })
                   .FillAsync("99999999");

        var depositForm = _page.Locator("form")
                       .Filter(new() { HasText = "Deposit" });

        await depositForm.GetByPlaceholder("Amount")
                         .FillAsync("200");


        await _page.GetByRole(AriaRole.Button, new() { Name = "Deposit" })
                   .ClickAsync();

        await Expect(_page.GetByText("Card not found"))
            .ToBeVisibleAsync();


        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Card Number" })
                   .FillAsync(cardNumber);

        await _page.GetByRole(AriaRole.Button, new() { Name = "Deposit" })
                   .ClickAsync();

        await Expect(_page.GetByText("Card not found"))
            .ToBeHiddenAsync();
    }
}
