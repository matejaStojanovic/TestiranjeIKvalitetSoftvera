using TransactionNow.Domain.Entities;
using MongoDB.Driver;
using TransactionNow.Application.Services;

namespace TransactionNow.Infrastructure;

public static class DatabaseSeeder
{
    public static void Seed(MongoContext context)
    {
        SeedUserWithCards(context, "Admin", "User", "admin@test.com", 10000000);
        SeedUserWithCards(context, "Marko", "Markovic", "user1@test.com", 5000000);
        SeedUserWithCards(context, "Ana", "Anic", "user2@test.com", 3000000);
    }

    private static void SeedUserWithCards(
        MongoContext context,
        string ime,
        string prezime,
        string email,
        decimal balance)
    {
        var user = context.Users
            .Find(u => u.Email == email)
            .FirstOrDefault();

        if (user == null)
        {
            user = new User
            {
                Ime = ime,
                Prezime = prezime,
                Email = email,
                PasswordHash = UserService.Hash("Test123!"),
                Balance = balance
            };

            context.Users.InsertOne(user);
        }

        SeedCard(context, user.Id, GenerateCardNumber(email, 1));
        SeedCard(context, user.Id, GenerateCardNumber(email, 2));
    }

    private static void SeedCard(
        MongoContext context,
        string userId,
        string cardNumber)
    {
        var existingCard = context.Cards
            .Find(c => c.CardNumber == cardNumber)
            .FirstOrDefault();

        if (existingCard != null)
            return;

        var card = new Card
        {
            UserId = userId,
            CardNumber = cardNumber
        };

        context.Cards.InsertOne(card);
    }

    private static string GenerateCardNumber(string email, int index)
    {
        var baseNumber = Math.Abs(email.GetHashCode()).ToString();
        baseNumber = baseNumber.PadLeft(12, '0');

        return baseNumber.Substring(0, 12) + index.ToString("0000");
    }
}
