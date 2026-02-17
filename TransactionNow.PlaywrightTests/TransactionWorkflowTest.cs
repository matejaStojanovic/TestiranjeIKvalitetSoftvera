using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Microsoft.Playwright.Assertions;

namespace TransactionNow.PlaywrightTests;

public class TransactionWorkflowTests : BaseTest
{
    private async Task<int> GetBalanceAsync()
    {
        var heading = _page.GetByRole(AriaRole.Heading)
                           .Filter(new() { HasText = "Balance:" });

        var text = await heading.InnerTextAsync();

        var match = Regex.Match(text, @"\d+");
        return int.Parse(match.Value);
    }

    [Test]
    public async Task SendMoney_Should_Update_Balances_Correctly()
    {
        int sendAmount = 3000;

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })
                   .FillAsync("user1@test.com");

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })
                   .FillAsync("Test123!");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" })
                   .ClickAsync();

        int user1BalanceBefore = await GetBalanceAsync();

        await _page.GetByRole(AriaRole.Button, new() { Name = "Logout" })
                   .ClickAsync();

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })
                   .FillAsync("admin@test.com");

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })
                   .FillAsync("Test123!");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" })
                   .ClickAsync();

        int adminBalanceBefore = await GetBalanceAsync();
        int higherBalanceTest = adminBalanceBefore * 10;
        sendAmount = adminBalanceBefore / 10;

        await _page.GetByRole(AriaRole.Link, new() { Name = "Transactions" })
                   .ClickAsync();

        var sendForm = _page.Locator("form")
                            .Filter(new() { HasText = "Send" });

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Recipient Email" })
                   .FillAsync("user1@test.com");

        await sendForm.GetByPlaceholder("Amount")
                      .FillAsync(higherBalanceTest.ToString());

        await sendForm.GetByRole(AriaRole.Button, new() { Name = "Send" })
                      .ClickAsync();

        await Expect(_page.GetByText("Insufficient funds"))
            .ToBeVisibleAsync();

        await sendForm.GetByPlaceholder("Amount")
                      .FillAsync("");

        await sendForm.GetByPlaceholder("Amount")
                      .FillAsync(sendAmount.ToString());

        await sendForm.GetByRole(AriaRole.Button, new() { Name = "Send" })
                      .ClickAsync();

        await _page.GetByRole(AriaRole.Link, new() { Name = "Dashboard" })
                   .ClickAsync();

        int adminBalanceAfter = await GetBalanceAsync();

        Assert.That(adminBalanceAfter,
            Is.EqualTo(adminBalanceBefore - sendAmount));

        await _page.GetByRole(AriaRole.Button, new() { Name = "Logout" })
                   .ClickAsync();

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })
                   .FillAsync("user1@test.com");

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })
                   .FillAsync("Test123!");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" })
                   .ClickAsync();

        int user1BalanceAfter = await GetBalanceAsync();

        Assert.That(user1BalanceAfter,
            Is.EqualTo(user1BalanceBefore + sendAmount));
    }


}
