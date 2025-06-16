using NUnit.Framework;
using tutorias.Models;
using tutorias.Backend.Authentication;
using Moq;

namespace NUnitTests
{
    public class AuthenticationLogicTest
    {
        [Test]
        public void login_ReturnsUserModel()
        {
            var repo = new Mock<IAuthenticationRepository>();
            var user = new UserModel();
            repo.Setup(r => r.login(user)).Returns(user);
            var logic = new AuthenticationLogic(repo.Object);
            var result = logic.login(user);
            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public void registerUser_ReturnsUserId()
        {
            var repo = new Mock<IAuthenticationRepository>();
            var user = new UserModel();
            repo.Setup(r => r.registerUser(user)).Returns(42);
            var logic = new AuthenticationLogic(repo.Object);
            var result = logic.registerUser(user);
            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void registerUser_ThrowsException_WhenEmailExists()
        {
            var repo = new Mock<IAuthenticationRepository>();
            var user = new UserModel { Email = "dup@mail.com" };
            repo.Setup(r => r.registerUser(user)).Throws(new InvalidOperationException("El correo dup@mail.com ya está asociado a un usuario en el sistema, debe usar otro"));
            var logic = new AuthenticationLogic(repo.Object);
            var ex = Assert.Throws<InvalidOperationException>(() => logic.registerUser(user));
            Assert.That(ex.Message, Does.Contain(user.Email));
        }
    }
}