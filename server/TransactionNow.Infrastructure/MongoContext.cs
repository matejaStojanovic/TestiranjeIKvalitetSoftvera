using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using TransactionNow.Domain.Entities;

namespace TransactionNow.Infrastructure;

public class MongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["Mongo:ConnectionString"]);
        _database = client.GetDatabase("TransactionNowDb");
    }

    public IMongoCollection<User> Users =>
        _database.GetCollection<User>("Users");

    public IMongoCollection<Transaction> Transactions =>
        _database.GetCollection<Transaction>("Transactions");

    public IMongoCollection<Card> Cards =>
        _database.GetCollection<Card>("Cards");

    public IMongoCollection<Invoice> Invoices =>
        _database.GetCollection<Invoice>("Invoices");
}
