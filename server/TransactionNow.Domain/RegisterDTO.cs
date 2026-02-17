namespace TransactionNow.Domain.Entities;
public class RegisterDTO
{
    public required string Ime { get; set; }
    public required string Prezime { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
