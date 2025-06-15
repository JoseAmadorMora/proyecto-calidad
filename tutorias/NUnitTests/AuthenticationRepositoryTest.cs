using NUnit.Framework;
using tutorias.Backend.Authentication;
using tutorias.Models;
using Moq;

namespace NUnitTests
{
    public class AuthenticationRepositoryTest
    {
        [Test]
        public void login_ReturnsUserModelOrNull()
        {
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.login(It.IsAny<UserModel>())).Returns((UserModel?)null);
            Assert.DoesNotThrow(() => repo.Object.login(new UserModel()));
        }

        [Test]
        public void registerUser_ReturnsInt()
        {
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.registerUser(It.IsAny<UserModel>())).Returns(1);
            Assert.DoesNotThrow(() => repo.Object.registerUser(new UserModel()));
        }
    }
}
