using TransactionNow.Domain.Entities;
using TransactionNow.Application.Interfaces;

namespace TransactionNow.Application.Services;

public class InvoiceService
{
    private readonly IUserRepository _userRepository;
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceService(
        IUserRepository userRepository,
        IInvoiceRepository invoiceRepository)
    {
        _userRepository = userRepository;
        _invoiceRepository = invoiceRepository;
    }


    public async Task SendInvoice(string senderId, string receiverEmail, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(senderId))
            throw new ArgumentException("Sender is required.");

        if (string.IsNullOrWhiteSpace(receiverEmail))
            throw new ArgumentException("Receiver email is required.");

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        var sender = await _userRepository.GetById(senderId);
        if (sender == null)
            throw new InvalidOperationException("Sender not found.");

        var receiver = await _userRepository.GetByEmail(receiverEmail);
        if (receiver == null)
            throw new InvalidOperationException("Receiver not found.");

        if (receiver.Id == sender.Id)
            throw new InvalidOperationException("Cannot send invoice to yourself.");

        var invoice = new Invoice
        {
            SenderId = sender.Id,
            ReceiverId = receiver.Id,
            Amount = amount,
            CreatedAt = DateTime.UtcNow,
            IsPaid = false
        };

        await _invoiceRepository.Add(invoice);
    }


    public async Task<List<Invoice>> GetInvoicesForUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User is required.");

        return await _invoiceRepository.GetByReceiverId(userId);
    }


    public async Task PayInvoice(string userId, string invoiceId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User is required.");

        var invoice = await _invoiceRepository.GetById(invoiceId);
        if (invoice == null)
            throw new InvalidOperationException("Invoice not found.");

        if (invoice.IsPaid)
            throw new InvalidOperationException("Invoice already paid.");

        if (invoice.ReceiverId != userId)
            throw new InvalidOperationException("Not authorized to pay this invoice.");

        var receiver = await _userRepository.GetById(invoice.ReceiverId);
        var sender = await _userRepository.GetById(invoice.SenderId);

        if (receiver == null || sender == null)
            throw new InvalidOperationException("User not found.");

        if (receiver.Balance < invoice.Amount)
            throw new InvalidOperationException("Insufficient funds.");


        receiver.Balance -= invoice.Amount;
        sender.Balance += invoice.Amount;

        invoice.IsPaid = true;

        await _userRepository.Update(receiver);
        await _userRepository.Update(sender);
        await _invoiceRepository.Update(invoice);
    }


    public async Task DeleteInvoice(string userId, string invoiceId)
    {
        var invoice = await _invoiceRepository.GetById(invoiceId);
        if (invoice == null)
            throw new InvalidOperationException("Invoice not found.");

        if (invoice.ReceiverId != userId)
            throw new InvalidOperationException("Not authorized to delete this invoice.");

        if (invoice.IsPaid)
            throw new InvalidOperationException("Cannot delete a paid invoice.");

        await _invoiceRepository.Delete(invoiceId);
    }
}
