using TransactionNow.Domain.Entities;

namespace TransactionNow.Application.Interfaces;

public interface IUserRepository
{
    Task<User> Create(User user);
    Task<User?> GetByEmail(string email);
    Task<User?> GetById(string id);
    Task Update(User user);
}
