using NUnit.Framework;
using tutorias.Backend.Authentication;
using tutorias.Models;
using Microsoft.Data.Sqlite;
using Dapper;

namespace NUnitTests
{
    public class AuthenticationRepositoryTest
    {
        private SqliteConnection _conn;
        private AuthenticationRepository _repo;

        [SetUp]
        public void Setup()
        {
            SQLitePCL.Batteries.Init();
            _conn = new SqliteConnection("Data Source=:memory:");
            _conn.Open();
            _conn.Execute("""
                CREATE TABLE Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Email TEXT,
                    Password TEXT,
                    UserType INTEGER
                );
            """);
            _repo = new AuthenticationRepository(_conn);
        }

        [TearDown]
        public void TearDown()
        {
            _conn.Dispose();
        }

        [Test]
        public void RegisterUser_ReturnsNewId()
        {
            var id = _repo.registerUser(new UserModel
            {
                Name = "Bob",
                Email = "bob@mail.com",
                Password = "pwd",
                UserType = UserTypes.Teacher
            });
            Assert.That(id, Is.GreaterThan(0));
        }

        [Test]
        public void RegisterUser_ThrowsException_WhenEmailExists()
        {
            var email = "bob@mail.com";
            _repo.registerUser(new UserModel
            {
                Name = "Bob",
                Email = email,
                Password = "pwd",
                UserType = UserTypes.Teacher
            });
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _repo.registerUser(new UserModel
                {
                    Name = "Otro",
                    Email = email,
                    Password = "pwd2",
                    UserType = UserTypes.Student
                })
            );
            Assert.That(ex.Message, Does.Contain(email));
        }

        [Test]
        public void Login_ReturnsUserModel_WhenCredentialsCorrect()
        {
            _repo.registerUser(new UserModel
            {
                Name = "Ana",
                Email = "ana@mail.com",
                Password = "123",
                UserType = UserTypes.Teacher
            });

            var user = _repo.login(new UserModel
            {
                Email = "ana@mail.com",
                Password = "123"
            });

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(user);
                Assert.AreEqual("Ana", user!.Name);
                Assert.AreEqual("ana@mail.com", user.Email);
            });
        }

        [Test]
        public void Login_ReturnsNull_WhenCredentialsWrong()
        {
            var result = _repo.login(new UserModel
            {
                Email = "does@not.exist",
                Password = "x"
            });
            Assert.IsNull(result);
        }
    }
}
