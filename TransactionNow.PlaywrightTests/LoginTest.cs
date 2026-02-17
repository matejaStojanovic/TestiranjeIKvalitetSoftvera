using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using static Microsoft.Playwright.Assertions;
using System.Text.RegularExpressions;

namespace TransactionNow.PlaywrightTests;

public class LoginTests : BaseTest
{
    [Test]
    public async Task Login_WithValidCredentials_ShouldNavigateToDashboard()
    {
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })
                   .FillAsync("admin@test.com");

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })
                   .FillAsync("Test123!");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" })
                   .ClickAsync();

        await Expect(_page).ToHaveURLAsync(new Regex(".*dashboard"));
    }

    [Test]
    public async Task Login_WithInvalidPassword_ShouldStayOnLoginPage_AndShowError()
    {
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })
                   .FillAsync("admin@test.com");

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })
                   .FillAsync("WrongPassword!");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" })
                   .ClickAsync();

        await Expect(_page).Not.ToHaveURLAsync(new Regex(".*dashboard"));

        await Expect(_page.GetByText("Something went wrong")).ToBeVisibleAsync();
    }

}

