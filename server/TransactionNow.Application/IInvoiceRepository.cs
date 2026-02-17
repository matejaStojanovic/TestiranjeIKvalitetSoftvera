using TransactionNow.Domain.Entities;

namespace TransactionNow.Application.Interfaces;

public interface IInvoiceRepository
{
    Task Add(Invoice invoice);
    Task Update(Invoice invoice);
    Task Delete(string invoiceId);
    Task<Invoice?> GetById(string invoiceId);
    Task<List<Invoice>> GetByReceiverId(string receiverId);
}
