using NUnit.Framework;
using Moq;
using TransactionNow.Application.Services;
using TransactionNow.Application.Interfaces;
using TransactionNow.Domain.Entities;

namespace TransactionNow.Tests.Services
{
    [TestFixture]
    public class TransactionServiceTests
    {
        private Mock<IUserRepository> _repositoryMock = null!;
        private TransactionService _service = null!;

        private User _sender = null!;
        private User _receiver = null!;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            TestContext.WriteLine("Starting TransactionService test suite...");
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _service = new TransactionService(_repositoryMock.Object);

            _sender = new User
            {
                Id = "1",
                Ime = "Marko",
                Prezime = "Markovic",
                Email = "marko@test.com",
                PasswordHash = "hash",
                Balance = 500
            };

            _receiver = new User
            {
                Id = "2",
                Ime = "Ana",
                Prezime = "Anic",
                Email = "ana@test.com",
                PasswordHash = "hash",
                Balance = 100
            };
        }

        [TearDown]
        public void Cleanup()
        {
            TestContext.WriteLine("Test finished.");
        }

        [OneTimeTearDown]
        public void GlobalCleanup()
        {
            TestContext.WriteLine("Finished TransactionService test suite.");
        }



        [Test]
        public async Task SendMoney_Should_Update_Balances_When_Valid()
        {
            _repositoryMock.Setup(r => r.GetById("1"))
                           .ReturnsAsync(_sender);

            _repositoryMock.Setup(r => r.GetByEmail("ana@test.com"))
                           .ReturnsAsync(_receiver);

            await _service.SendMoney("1", "ana@test.com", 200);

            Assert.That(_sender.Balance, Is.EqualTo(300));
            Assert.That(_receiver.Balance, Is.EqualTo(300));
            Assert.That(_sender.SentTransactions, Has.Count.EqualTo(1));
            Assert.That(_receiver.ReceivedTransactions, Has.Count.EqualTo(1));
        }

        [Test]
        public void SendMoney_Should_Throw_When_Insufficient_Funds()
        {
            _sender.Balance = 50;

            _repositoryMock.Setup(r => r.GetById("1"))
                           .ReturnsAsync(_sender);

            _repositoryMock.Setup(r => r.GetByEmail("ana@test.com"))
                           .ReturnsAsync(_receiver);

            Assert.That(async () =>
                await _service.SendMoney("1", "ana@test.com", 200),
                Throws.TypeOf<InvalidOperationException>()
                      .With.Message.EqualTo("Insufficient funds."));
        }

        [Test]
        public void SendMoney_Should_Throw_When_Sending_To_Self()
        {
            _repositoryMock.Setup(r => r.GetById("1"))
                           .ReturnsAsync(_sender);

            _repositoryMock.Setup(r => r.GetByEmail("marko@test.com"))
                           .ReturnsAsync(_sender);

            Assert.That(async () =>
                await _service.SendMoney("1", "marko@test.com", 100),
                Throws.TypeOf<ArgumentException>()
                      .With.Message.EqualTo("Cannot send money to yourself."));
        }

        [Test]
        public void SendMoney_Should_Throw_When_Amount_Is_Invalid()
        {
            Assert.That(async () =>
                await _service.SendMoney("1", "ana@test.com", -10),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void SendMoney_Should_Throw_When_Recipient_Not_Found()
        {
            _repositoryMock.Setup(r => r.GetById("1"))
                           .ReturnsAsync(_sender);

            _repositoryMock.Setup(r => r.GetByEmail("ana@test.com"))
                           .ReturnsAsync((User?)null);

            Assert.That(async () =>
                await _service.SendMoney("1", "ana@test.com", 100),
                Throws.TypeOf<InvalidOperationException>()
                      .With.Message.EqualTo("Recipient not found."));
        }

        [Test]
        [Ignore("Atomicity test - implement transaction rollback logic later")]
        public async Task SendMoney_Should_Not_Modify_Balances_When_Exception()
        {
            _sender.Balance = 50;

            _repositoryMock.Setup(r => r.GetById("1"))
                           .ReturnsAsync(_sender);

            _repositoryMock.Setup(r => r.GetByEmail("ana@test.com"))
                           .ReturnsAsync(_receiver);

            try
            {
                await _service.SendMoney("1", "ana@test.com", 200);
            }
            catch { }

            Assert.That(_sender.Balance, Is.EqualTo(50));
            Assert.That(_receiver.Balance, Is.EqualTo(100));
        }
    }
}
