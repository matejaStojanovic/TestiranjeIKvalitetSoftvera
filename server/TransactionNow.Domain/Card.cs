namespace TransactionNow.Domain.Entities;

public class Card
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string UserId { get; set; }
    public User User { get; set; } = null!;

    public required string CardNumber { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
