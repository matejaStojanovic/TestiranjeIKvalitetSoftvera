using TransactionNow.Domain.Entities;

namespace TransactionNow.Application.Interfaces;

public interface ITransactionRepository
{
    Task<bool> Transfer(string fromId, string toId, decimal amount);
}
