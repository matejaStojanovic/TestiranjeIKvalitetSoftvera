using MongoDB.Driver;
using TransactionNow.Domain.Entities;
using TransactionNow.Application.Interfaces;

namespace TransactionNow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MongoContext _context;

    public UserRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task<User> Create(User user)
    {
        await _context.Users.InsertOneAsync(user);
        return user;
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _context.Users
            .Find(u => u.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetById(string id)
    {
        return await _context.Users
            .Find(u => u.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task Update(User user)
    {
        await _context.Users
            .ReplaceOneAsync(u => u.Id == user.Id, user);
    }
}
