public class Invoice
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SenderId { get; set; } = null!;
    public string ReceiverId { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPaid { get; set; } = false;
}
