using TransactionNow.Domain.Entities;
using TransactionNow.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace TransactionNow.Application.Services;

public class UserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<User> Register(
        string ime,
        string prezime,
        string email,
        string password)
    {
        var existing = await _repository.GetByEmail(email);
        if (existing != null)
            throw new Exception("User with this email already exists.");

        var hash = Hash(password);

        var user = new User
        {
            Ime = ime,
            Prezime = prezime,
            Email = email,
            PasswordHash = hash,
            Balance = 0
        };

        return await _repository.Create(user);
    }

    public async Task<User?> Login(string email, string password)
    {
        var user = await _repository.GetByEmail(email);
        if (user == null)
            return null;

        var hash = Hash(password);

        if (user.PasswordHash != hash)
            return null;

        return user;
    }

    public static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}
