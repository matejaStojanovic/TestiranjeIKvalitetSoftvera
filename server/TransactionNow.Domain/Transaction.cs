namespace TransactionNow.Domain.Entities;

public class Transaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string FromUserId { get; set; }
    public User FromUser { get; set; } = null!;

    public string ToUserId { get; set; } = null!;
    public User ToUser { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    public string Status { get; set; } = null!; // Success / Failed
}
