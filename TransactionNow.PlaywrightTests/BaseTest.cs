using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TransactionNow.PlaywrightTests;

public class BaseTest
{
    protected IPlaywright _playwright;
    protected IBrowser _browser;
    protected IBrowserContext _context;
    protected IPage _page;

    protected string FrontendUrl = "http://localhost:3000";

    [SetUp]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();

        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false // 
        });

        _context = await _browser.NewContextAsync();

        _page = await _context.NewPageAsync();

        await _page.GotoAsync(FrontendUrl);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _browser.CloseAsync();
        _playwright.Dispose();
    }
}
