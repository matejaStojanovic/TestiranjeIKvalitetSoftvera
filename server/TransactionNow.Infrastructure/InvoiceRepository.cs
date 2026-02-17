using MongoDB.Driver;
using TransactionNow.Application.Interfaces;
using TransactionNow.Domain.Entities;

namespace TransactionNow.Infrastructure.Repositories;

public class MongoInvoiceRepository : IInvoiceRepository
{
    private readonly IMongoCollection<Invoice> _collection;

    public MongoInvoiceRepository(MongoContext context)
    {
        _collection = context.Invoices;
    }

    public async Task Add(Invoice invoice)
    {
        await _collection.InsertOneAsync(invoice);
    }

    public async Task Update(Invoice invoice)
    {
        await _collection.ReplaceOneAsync(
            i => i.Id == invoice.Id,
            invoice
        );
    }

    public async Task Delete(string invoiceId)
    {
        await _collection.DeleteOneAsync(
            i => i.Id == invoiceId
        );
    }

    public async Task<Invoice?> GetById(string invoiceId)
    {
        return await _collection
            .Find(i => i.Id == invoiceId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Invoice>> GetByReceiverId(string receiverId)
    {
        return await _collection
            .Find(i => i.ReceiverId == receiverId && !i.IsPaid)
            .ToListAsync();
    }
}
