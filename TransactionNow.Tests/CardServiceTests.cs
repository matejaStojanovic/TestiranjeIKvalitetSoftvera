using NUnit.Framework;
using Moq;
using TransactionNow.Application.Services;
using TransactionNow.Application.Interfaces;
using TransactionNow.Domain.Entities;
using TransactionNow.Tests.CustomConstraints;

namespace TransactionNow.Tests.Services
{
    [TestFixture]
    public class CardServiceTests
    {
        private Mock<IUserRepository> _repositoryMock = null!;
        private CardService _service = null!;
        private User _user = null!;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            TestContext.WriteLine("Starting CardService test suite...");
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _service = new CardService(_repositoryMock.Object);

            _user = new User
            {
                Id = "1",
                Ime = "Marko",
                Prezime = "Markovic",
                Email = "marko@test.com",
                PasswordHash = "hash",
                Balance = 0
            };
        }


        [TearDown]
        public void Cleanup()
        {
            TestContext.WriteLine("CardService test finished.");
        }


        [OneTimeTearDown]
        public void GlobalCleanup()
        {
            TestContext.WriteLine("Finished CardService test suite.");
        }



        [Test]
        public async Task AddCard_Should_Add_Card_When_Valid()
        {
            _repositoryMock.Setup(r => r.GetById("1"))
                           .ReturnsAsync(_user);

            await _service.AddCard("1", "12345678");

            Assert.That(_user.Cards,
                Has.Count.EqualTo(1)
                & Has.Some.Property("CardNumber").EqualTo("12345678")
            );
        }
        [Test]
        public void CardNumber_Should_Be_Valid_When_Format_Is_Correct()
        {
            string cardNumber = "12345678";

            Assert.That(cardNumber, new ValidCardNumberConstraint());
        }

        [Test]
        public void AddCard_Should_Throw_When_UserId_Is_Invalid()
        {
            Assert.That(async () =>
                await _service.AddCard("", "12345678"),
                Throws.TypeOf<ArgumentException>()
            );
        }

        [Test]
        public void AddCard_Should_Throw_When_CardNumber_Is_Invalid()
        {
            Assert.That(async () =>
                await _service.AddCard("1", "123"),
                Throws.TypeOf<ArgumentException>()
            );
        }

        [Test]
        public async Task AddCard_Should_Throw_When_Card_Already_Exists()
        {
            _user.Cards.Add(new Card
            {
                Id = "card1",
                UserId = "1",
                CardNumber = "12345678"
            });

            _repositoryMock.Setup(r => r.GetById("1"))
                           .ReturnsAsync(_user);

            Assert.That(async () =>
                await _service.AddCard("1", "12345678"),
                Throws.TypeOf<InvalidOperationException>()
            );
        }


        [Test]
        public void AddCard_Should_Throw_When_User_Not_Found()
        {
            _repositoryMock.Setup(r => r.GetById("1"))
                           .ReturnsAsync((User?)null);

            Assert.That(async () =>
                await _service.AddCard("1", "12345678"),
                Throws.TypeOf<InvalidOperationException>()
                      .With.Message.EqualTo("User not found.")
            );
        }

        [Test]
        public async Task RemoveCard_Should_Remove_Card_When_Exists()
        {
            var card = new Card
            {
                Id = "card1",
                UserId = "1",
                CardNumber = "12345678"
            };

            _user.Cards.Add(card);

            _repositoryMock.Setup(r => r.GetById("1"))
                           .ReturnsAsync(_user);

            await _service.RemoveCard("1", "card1");

            Assert.That(_user.Cards, Is.Empty);
        }

        [Test]
        public void RemoveCard_Should_Throw_When_Card_Not_Found()
        {
            _repositoryMock.Setup(r => r.GetById("1"))
                           .ReturnsAsync(_user);

            Assert.That(async () =>
                await _service.RemoveCard("1", "nonexistent"),
                Throws.TypeOf<Exception>()
                      .With.Message.EqualTo("Card not found")
            );
        }

        [Test]
        public void RemoveCard_Should_Throw_When_User_Not_Found()
        {
            _repositoryMock.Setup(r => r.GetById("1"))
                           .ReturnsAsync((User?)null);

            Assert.That(async () =>
                await _service.RemoveCard("1", "card1"),
                Throws.TypeOf<Exception>()
                      .With.Message.EqualTo("User not found")
            );
        }

        [Test]
        [Ignore("Future feature: prevent duplicate card numbers")]
        public async Task AddCard_Should_Prevent_Duplicate_CardNumbers()
        {
            await _service.AddCard("1", "12345678");
        }
    }
}
