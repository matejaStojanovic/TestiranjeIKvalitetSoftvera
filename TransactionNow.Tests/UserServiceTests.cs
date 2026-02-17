using NUnit.Framework;
using Moq;
using TransactionNow.Application.Services;
using TransactionNow.Application.Interfaces;
using TransactionNow.Domain.Entities;

namespace TransactionNow.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _repositoryMock = null!;
        private UserService _service = null!;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            TestContext.WriteLine("Starting UserService test suite...");
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _service = new UserService(_repositoryMock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            TestContext.WriteLine("UserService test finished.");
        }


        [OneTimeTearDown]
        public void GlobalCleanup()
        {
            TestContext.WriteLine("Finished UserService test suite.");
        }



        [Test]
        public async Task Register_Should_Create_User_When_Email_Not_Exists()
        {
            _repositoryMock.Setup(r => r.GetByEmail("test@test.com"))
                           .ReturnsAsync((User?)null);

            _repositoryMock.Setup(r => r.Create(It.IsAny<User>()))
                           .ReturnsAsync((User u) => u);

            var result = await _service.Register(
                "Marko",
                "Markovic",
                "test@test.com",
                "Password123"
            );

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo("test@test.com"));
            Assert.That(result.PasswordHash, Is.Not.EqualTo("Password123"));
        }
        [Test]
        public async Task Register_Should_Set_Hash_And_Email_Correctly()
        {
            _repositoryMock.Setup(r => r.GetByEmail("test@test.com"))
                           .ReturnsAsync((User?)null);

            _repositoryMock.Setup(r => r.Create(It.IsAny<User>()))
                           .ReturnsAsync((User u) => u);

            var result = await _service.Register(
                "Marko",
                "Markovic",
                "test@test.com",
                "Password123"
            );

            Assert.That(result,
                Has.Property("Email").EqualTo("test@test.com")
                & Has.Property("PasswordHash").Not.EqualTo("Password123")
            );
        }

        [Test]
        public void Register_Should_Throw_When_Email_Already_Exists()
        {
            var existingUser = new User
            {
                Ime = "Ana",
                Prezime = "Anic",
                Email = "test@test.com",
                PasswordHash = "hash"
            };

            _repositoryMock.Setup(r => r.GetByEmail("test@test.com"))
                           .ReturnsAsync(existingUser);

            Assert.That(async () =>
                await _service.Register(
                    "Marko",
                    "Markovic",
                    "test@test.com",
                    "Password123"
                ),
                Throws.TypeOf<Exception>()
                      .With.Message.EqualTo("User with this email already exists."));
        }

        [Test]
        public async Task Login_Should_Return_User_When_Credentials_Are_Valid()
        {
            var user = new User
            {
                Ime = "Marko",
                Prezime = "Markovic",
                Email = "login@test.com",
                PasswordHash = UserService.Hash("Password123")
            };

            _repositoryMock.Setup(r => r.GetByEmail("login@test.com"))
                           .ReturnsAsync(user);

            var result = await _service.Login("login@test.com", "Password123");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Email, Is.EqualTo("login@test.com"));
        }

        [Test]
        public async Task Login_Should_Return_Null_When_Password_Is_Wrong()
        {
            var user = new User
            {
                Ime = "Marko",
                Prezime = "Markovic",
                Email = "login@test.com",
                PasswordHash = UserService.Hash("CorrectPassword")
            };

            _repositoryMock.Setup(r => r.GetByEmail("login@test.com"))
                           .ReturnsAsync(user);

            var result = await _service.Login("login@test.com", "WrongPassword");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Login_Should_Return_Null_When_User_Not_Found()
        {
            _repositoryMock.Setup(r => r.GetByEmail("unknown@test.com"))
                           .ReturnsAsync((User?)null);

            var result = await _service.Login("unknown@test.com", "Password123");

            Assert.That(result, Is.Null);
        }

        [Test]
        [Ignore("Future enhancement: password complexity validation")]
        public async Task Register_Should_Validate_Password_Strength()
        {
            await _service.Register("A", "B", "x@test.com", "123");
        }
    }
}
