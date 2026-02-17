namespace TransactionNow.Domain.Entities;
public class NovaTransakcijaDTO
{
    public string RecipientEmail { get; set; } = string.Empty;
    public decimal Iznos { get; set; }
}
