using Microsoft.Playwright;
using NUnit.Framework;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace TransactionNow.PlaywrightTests;

public class ApiTests
{
    private IPlaywright _playwright;
    private IAPIRequestContext _request;

    private string BaseUrl = "http://localhost:5295";

    [SetUp]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();

        _request = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = BaseUrl,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            }
        });
    }

    [TearDown]
    public async Task TearDown()
    {
        await _request.DisposeAsync();
        _playwright.Dispose();
    }


    private async Task LoginAsAdmin()
    {
        var response = await _request.PostAsync("/api/auth/login", new()
        {
            DataObject = new
            {
                email = "admin@test.com",
                password = "Test123!"
            }
        });

        Assert.That(response.Status, Is.EqualTo((int)HttpStatusCode.OK));
    }


    [Test]
    public async Task Login_Ispravno()
    {
        var response = await _request.PostAsync("/api/auth/login", new()
        {
            DataObject = new
            {
                email = "admin@test.com",
                password = "Test123!"
            }
        });

        Assert.That(response.Status, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(response.Headers.ContainsKey("set-cookie"), Is.True);
    }

    [Test]
    public async Task Login_Negativan()
    {
        var response = await _request.PostAsync("/api/auth/login", new()
        {
            DataObject = new
            {
                email = "admin@test.com",
                password = "POGRESNA"
            }
        });

        Assert.That(response.Status, Is.Not.EqualTo((int)HttpStatusCode.OK));
    }



    [Test]
    public async Task Register_WithExistingEmail_ShouldReturnError()
    {
        var response = await _request.PostAsync("/api/auth/register", new()
        {
            DataObject = new
            {
                firstName = "Test",
                lastName = "User",
                email = "user1@test.com",
                password = "Test123!"
            }
        });

        Assert.That(response.Status, Is.Not.EqualTo((int)HttpStatusCode.OK));
    }




    [Test]
    [Ignore("Admin vec ima ovu karticu.")] 
    public async Task AddCard_ShouldReturn200()
    {
        await LoginAsAdmin();

        var response = await _request.PostAsync("/api/cards", new()
        {
            DataObject = new
            {
                brojKartice = "55556666"
            }
        });

        Assert.That(response.Status, Is.EqualTo((int)HttpStatusCode.OK));
    }

    [Test]
    public async Task AddCard_Duplicate_ShouldReturn409()
    {
        await LoginAsAdmin();

        string cardNumber = "88889999";

        await _request.PostAsync("/api/cards", new()
        {
            DataObject = new { brojKartice = cardNumber }
        });

        var response = await _request.PostAsync("/api/cards", new()
        {
            DataObject = new { brojKartice = cardNumber }
        });

        Assert.That(response.Status, Is.EqualTo((int)HttpStatusCode.Conflict));
    }

    [Test]
    public async Task GetCards_ShouldReturnList()
    {
        await LoginAsAdmin();

        var response = await _request.GetAsync("/api/cards");

        Assert.That(response.Status, Is.EqualTo((int)HttpStatusCode.OK));

        var body = await response.TextAsync();
        Assert.That(body, Is.Not.Null);
    }

    [Test]
    public async Task DeleteCard_ShouldReturn200()
    {
        await LoginAsAdmin();

        string cardNumber = "11112222";

        await _request.PostAsync("/api/cards", new()
        {
            DataObject = new { brojKartice = cardNumber }
        });

        var cardsResponse = await _request.GetAsync("/api/cards");
        var cardsJson = await cardsResponse.TextAsync();

        using var doc = JsonDocument.Parse(cardsJson);
        var cards = doc.RootElement;

        var cardId = cards.EnumerateArray()
                          .First(c => c.GetProperty("cardNumber").GetString() == cardNumber)
                          .GetProperty("id")
                          .GetString();

        var deleteResponse = await _request.DeleteAsync($"/api/cards/{cardId}");

        Assert.That(deleteResponse.Status, Is.EqualTo((int)HttpStatusCode.OK));
    }



    [Test]
    [Ignore("nema kartice u seed")]
    public async Task Deposit_ShouldIncreaseBalance()
    {
        await LoginAsAdmin();

        var response = await _request.PostAsync("/api/transactions/deposit", new()
        {
            DataObject = new
            {
                cardNumber = "55556666",
                amount = 500
            }
        });

        Assert.That(response.Status, Is.EqualTo((int)HttpStatusCode.OK));
    }

    [Test]
    public async Task Send_WithInsufficientFunds_ShouldFail()
    {
        await LoginAsAdmin();

        var response = await _request.PostAsync("/api/transactions", new()
        {
            DataObject = new
            {
                recipientEmail = "user1@test.com",
                amount = 99999999
            }
        });

        Assert.That(response.Status, Is.Not.EqualTo((int)HttpStatusCode.OK));
    }


    [Test]
    public async Task SendInvoice_ShouldReturn200()
    {
        await LoginAsAdmin();

        var response = await _request.PostAsync("/api/invoices", new()
        {
            DataObject = new
            {
                receiverEmail = "user1@test.com",
                amount = 500
            }
        });

        Assert.That(response.Status, Is.EqualTo((int)HttpStatusCode.OK));
    }

    [Test]
    public async Task SendInvoice_ToNonExistingUser_ShouldFail()
    {
        await LoginAsAdmin();

        var response = await _request.PostAsync("/api/invoices", new()
        {
            DataObject = new
            {
                receiverEmail = "nepostojeci@test.com",
                amount = 500
            }
        });

        Assert.That(response.Status, Is.Not.EqualTo((int)HttpStatusCode.OK));
    }

    [Test]
    public async Task GetInvoices_ShouldReturnList()
    {
        await LoginAsAdmin();

        var response = await _request.GetAsync("/api/invoices");

        Assert.That(response.Status, Is.EqualTo((int)HttpStatusCode.OK));

        var body = await response.TextAsync();
        Assert.That(body, Is.Not.Null);
    }

    [Test]
    public async Task PayInvoice_ShouldTransferMoney()
    {
        await LoginAsAdmin();

        await _request.PostAsync("/api/invoices", new()
        {
            DataObject = new
            {
                receiverEmail = "user1@test.com",
                amount = 300
            }
        });

        var loginUser = await _request.PostAsync("/api/auth/login", new()
        {
            DataObject = new
            {
                email = "user1@test.com",
                password = "Test123!"
            }
        });

        Assert.That(loginUser.Status, Is.EqualTo((int)HttpStatusCode.OK));

        var invoicesResponse = await _request.GetAsync("/api/invoices");
        var invoicesJson = await invoicesResponse.TextAsync();

        using var doc = JsonDocument.Parse(invoicesJson);
        var invoices = doc.RootElement;

        var invoiceId = invoices.EnumerateArray()
                                .First()
                                .GetProperty("id")
                                .GetString();

        var payResponse = await _request.PostAsync($"/api/invoices/{invoiceId}/pay");

        Assert.That(payResponse.Status, Is.EqualTo((int)HttpStatusCode.OK));
    }

    [Test]
    public async Task DeleteInvoice_ShouldReturn200()
    {
        await LoginAsAdmin();

        await _request.PostAsync("/api/invoices", new()
        {
            DataObject = new
            {
                receiverEmail = "user1@test.com",
                amount = 200
            }
        });

        await _request.PostAsync("/api/auth/login", new()
        {
            DataObject = new
            {
                email = "user1@test.com",
                password = "Test123!"
            }
        });

        var invoicesResponse = await _request.GetAsync("/api/invoices");
        var invoicesJson = await invoicesResponse.TextAsync();

        using var doc = JsonDocument.Parse(invoicesJson);
        var invoices = doc.RootElement;

        var invoiceId = invoices.EnumerateArray()
                                .First()
                                .GetProperty("id")
                                .GetString();

        var deleteResponse = await _request.DeleteAsync($"/api/invoices/{invoiceId}");

        Assert.That(deleteResponse.Status, Is.EqualTo((int)HttpStatusCode.OK));
    }
}
