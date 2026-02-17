using TransactionNow.Domain.Entities;
using TransactionNow.Application.Interfaces;

namespace TransactionNow.Application.Services;

public class TransactionService
{
    private readonly IUserRepository _repository;

    public TransactionService(IUserRepository repository)
    {
        _repository = repository;
    }
    public async Task<List<Transaction>> GetUserTransactions(string userId)
    {
        var user = await _repository.GetById(userId);
        if (user == null)
            throw new Exception("User not found");

        return user.SentTransactions
            .Concat(user.ReceivedTransactions)
            .OrderByDescending(t => t.DateTime)
            .ToList();
    }
    public async Task Deposit(string userId, string cardNumber, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User is required.");

        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentException("Card number is required.");

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        var user = await _repository.GetById(userId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var card = user.Cards
            .FirstOrDefault(c => c.CardNumber == cardNumber);

        if (card == null)
            throw new InvalidOperationException("Card not found.");

        user.Balance += amount;

        var transaction = new Transaction
        {
            FromUserId = user.Id,
            ToUserId = user.Id,
            Amount = amount,
            DateTime = DateTime.UtcNow,
            Status = "Deposit"
        };

        user.ReceivedTransactions.Add(transaction);

        await _repository.Update(user);
    }



    public async Task SendMoney(string fromUserId, string recipientEmail, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(fromUserId))
            throw new ArgumentException("Sender is required.");

        if (string.IsNullOrWhiteSpace(recipientEmail))
            throw new ArgumentException("Recipient email is required.");

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        var fromUser = await _repository.GetById(fromUserId);
        if (fromUser == null)
            throw new InvalidOperationException("Sender not found.");

        var toUser = await _repository.GetByEmail(recipientEmail);
        if (toUser == null)
            throw new InvalidOperationException("Recipient not found.");

        if (fromUser.Id == toUser.Id)
            throw new ArgumentException("Cannot send money to yourself.");

        if (fromUser.Balance < amount)
            throw new InvalidOperationException("Insufficient funds.");

        fromUser.Balance -= amount;
        toUser.Balance += amount;

        var transaction = new Transaction
        {
            FromUserId = fromUser.Id,
            ToUserId = toUser.Id,
            Amount = amount,
            DateTime = DateTime.UtcNow,
            Status = "Success"
        };

        fromUser.SentTransactions.Add(transaction);
        toUser.ReceivedTransactions.Add(transaction);

        await _repository.Update(fromUser);
        await _repository.Update(toUser);
    }


}