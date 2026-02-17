using MongoDB.Driver;
using TransactionNow.Application.Interfaces;
using TransactionNow.Domain.Entities;

namespace TransactionNow.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly MongoContext _context;

    public TransactionRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task<bool> Transfer(string fromId, string toId, decimal amount)
    {
        var fromUser = await _context.Users
            .Find(x => x.Id == fromId)
            .FirstOrDefaultAsync();

        var toUser = await _context.Users
            .Find(x => x.Id == toId)
            .FirstOrDefaultAsync();

        if (fromUser == null || toUser == null)
            return false;

        if (fromUser.Balance < amount)
            return false;

        fromUser.Balance -= amount;
        toUser.Balance += amount;

        await _context.Users.ReplaceOneAsync(x => x.Id == fromId, fromUser);
        await _context.Users.ReplaceOneAsync(x => x.Id == toId, toUser);

        await _context.Transactions.InsertOneAsync(new Transaction
        {
            FromUserId = fromId,
            ToUserId = toId,
            Amount = amount
        });

        return true;
    }
}
