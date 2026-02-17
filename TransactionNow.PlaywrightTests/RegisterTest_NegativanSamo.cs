using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Microsoft.Playwright.Assertions;

namespace TransactionNow.PlaywrightTests;

public class RegisterTests : BaseTest
{
    [Test]
    public async Task Register_WithExisting()
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Register" })
                   .ClickAsync();

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Ime", Exact = true })
                   .FillAsync("Postojeci");

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Prezime" })
                   .FillAsync("Korisnik");

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" })
                   .FillAsync("user1@test.com");

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })
                   .FillAsync("Test123!");

        await _page.GetByRole(AriaRole.Button, new() { Name = "Register" })
                   .ClickAsync();

        await Expect(_page).ToHaveURLAsync(new Regex(".*register"));

        await Expect(_page.GetByText("User with this email already"))
            .ToBeVisibleAsync();
    }
}
