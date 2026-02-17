namespace TransactionNow.Domain.Entities;
public class DepositDTO
{
    public string CardNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
