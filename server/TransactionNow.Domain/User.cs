namespace TransactionNow.Domain.Entities;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string Ime { get; set; }
    public required string Prezime { get; set; }

    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    public decimal Balance { get; set; } = 0;

    public List<Card> Cards { get; set; } = new();
    public List<Transaction> SentTransactions { get; set; } = new();
    public List<Transaction> ReceivedTransactions { get; set; } = new();
}
