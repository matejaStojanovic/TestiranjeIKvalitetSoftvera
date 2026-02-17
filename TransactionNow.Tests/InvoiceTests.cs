using NUnit.Framework;
using Moq;
using TransactionNow.Application.Services;
using TransactionNow.Application.Interfaces;
using TransactionNow.Domain.Entities;

namespace TransactionNow.Tests.Services;

[TestFixture]
public class InvoiceServiceTests
{
    private Mock<IUserRepository> _userRepoMock = null!;
    private Mock<IInvoiceRepository> _invoiceRepoMock = null!;
    private InvoiceService _service = null!;

    private User _sender = null!;
    private User _receiver = null!;

    [SetUp]
    public void Setup()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _invoiceRepoMock = new Mock<IInvoiceRepository>();

        _service = new InvoiceService(
            _userRepoMock.Object,
            _invoiceRepoMock.Object
        );

        _sender = new User
            {
                Id = "1",
                Ime = "Marko",
                Prezime = "Markovic",
                Email = "marko@test.com",
                PasswordHash = "hash",
                Balance = 10000
            };

            _receiver = new User
            {
                Id = "2",
                Ime = "Ana",
                Prezime = "Anic",
                Email = "ana@test.com",
                PasswordHash = "hash",
                Balance = 5000
            };
    }

    // -----------------------------------------
    // SEND INVOICE
    // -----------------------------------------

    [Test]
    public async Task SendInvoice_Should_Create_Invoice_When_Valid()
    {
        _userRepoMock.Setup(r => r.GetById("1"))
                     .ReturnsAsync(_sender);

        _userRepoMock.Setup(r => r.GetByEmail("user@test.com"))
                     .ReturnsAsync(_receiver);

        await _service.SendInvoice("1", "user@test.com", 500);

        _invoiceRepoMock.Verify(r =>
            r.Add(It.Is<Invoice>(i =>
                i.SenderId == "1" &&
                i.ReceiverId == "2" &&
                i.Amount == 500 &&
                i.IsPaid == false
            )),
            Times.Once);
    }

    [Test]
    public void SendInvoice_Should_Throw_When_Receiver_Not_Found()
    {
        _userRepoMock.Setup(r => r.GetById("1"))
                     .ReturnsAsync(_sender);

        _userRepoMock.Setup(r => r.GetByEmail("unknown@test.com"))
                     .ReturnsAsync((User?)null);

        Assert.That(async () =>
            await _service.SendInvoice("1", "unknown@test.com", 100),
            Throws.TypeOf<InvalidOperationException>()
                  .With.Message.EqualTo("Receiver not found.")
        );
    }

    // -----------------------------------------
    // PAY INVOICE
    // -----------------------------------------

    [Test]
    public async Task PayInvoice_Should_Transfer_Balance_When_Valid()
    {
        var invoice = new Invoice
        {
            Id = "inv1",
            SenderId = "1",
            ReceiverId = "2",
            Amount = 1000,
            IsPaid = false
        };

        _invoiceRepoMock.Setup(r => r.GetById("inv1"))
                        .ReturnsAsync(invoice);

        _userRepoMock.Setup(r => r.GetById("2"))
                     .ReturnsAsync(_receiver);

        _userRepoMock.Setup(r => r.GetById("1"))
                     .ReturnsAsync(_sender);

        await _service.PayInvoice("2", "inv1");

        Assert.That(_receiver.Balance, Is.EqualTo(4000));
        Assert.That(_sender.Balance, Is.EqualTo(11000));
        Assert.That(invoice.IsPaid, Is.True);

        _invoiceRepoMock.Verify(r => r.Update(invoice), Times.Once);
    }

    [Test]
    public void PayInvoice_Should_Throw_When_Insufficient_Funds()
    {
        _receiver.Balance = 100;

        var invoice = new Invoice
        {
            Id = "inv1",
            SenderId = "1",
            ReceiverId = "2",
            Amount = 1000,
            IsPaid = false
        };

        _invoiceRepoMock.Setup(r => r.GetById("inv1"))
                        .ReturnsAsync(invoice);

        _userRepoMock.Setup(r => r.GetById("2"))
                     .ReturnsAsync(_receiver);

        _userRepoMock.Setup(r => r.GetById("1"))
                     .ReturnsAsync(_sender);

        Assert.That(async () =>
            await _service.PayInvoice("2", "inv1"),
            Throws.TypeOf<InvalidOperationException>()
                  .With.Message.EqualTo("Insufficient funds.")
        );
    }

    [Test]
    public void PayInvoice_Should_Throw_When_Already_Paid()
    {
        var invoice = new Invoice
        {
            Id = "inv1",
            SenderId = "1",
            ReceiverId = "2",
            Amount = 1000,
            IsPaid = true
        };

        _invoiceRepoMock.Setup(r => r.GetById("inv1"))
                        .ReturnsAsync(invoice);

        Assert.That(async () =>
            await _service.PayInvoice("2", "inv1"),
            Throws.TypeOf<InvalidOperationException>()
                  .With.Message.EqualTo("Invoice already paid.")
        );
    }

    // -----------------------------------------
    // DELETE
    // -----------------------------------------

    [Test]
    public async Task DeleteInvoice_Should_Delete_When_Valid()
    {
        var invoice = new Invoice
        {
            Id = "inv1",
            ReceiverId = "2",
            IsPaid = false
        };

        _invoiceRepoMock.Setup(r => r.GetById("inv1"))
                        .ReturnsAsync(invoice);

        await _service.DeleteInvoice("2", "inv1");

        _invoiceRepoMock.Verify(r => r.Delete("inv1"), Times.Once);
    }
}
