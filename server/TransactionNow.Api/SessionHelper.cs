namespace TransactionNow.Api.Helpers;

public static class SessionHelper
{
    public static string? GetUserId(HttpContext context)
    {
        return context.Session.GetString("UserId");
    }
}
