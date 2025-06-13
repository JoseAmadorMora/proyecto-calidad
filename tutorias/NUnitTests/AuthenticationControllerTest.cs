using NUnit.Framework;
using tutorias.Features.Authentication;
using tutorias.Backend.Authentication;
using tutorias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace NUnitTests
{
    public class AuthenticationControllerTest
    {
        AuthenticationController GetControllerWithRepoMock(Mock<IAuthenticationRepository> repoMock)
        {
            var logic = new AuthenticationLogic(repoMock.Object);
            var controller = new AuthenticationController(logic);
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            var tempDataProvider = new Mock<ITempDataProvider>();
            controller.TempData = new TempDataDictionary(httpContext, tempDataProvider.Object);
            return controller;
        }

        [Test]
        public void LoginPage_ReturnsViewResult()
        {
            var repo = new Mock<IAuthenticationRepository>();
            var controller = GetControllerWithRepoMock(repo);
            var result = controller.LoginPage();
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public void RegisterPage_ReturnsViewResult()
        {
            var repo = new Mock<IAuthenticationRepository>();
            var controller = GetControllerWithRepoMock(repo);
            var result = controller.RegisterPage();
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public void login_ReturnsContentResult_ForStudent()
        {
            var user = new UserModel { Name = "Estudiante", UserType = UserTypes.Student, Id = 1, Email = "a@a.com" };
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.login(It.IsAny<UserModel>())).Returns(user);
            var controller = GetControllerWithRepoMock(repo);
            var result = controller.login(new UserModel());
            Assert.That(result, Is.InstanceOf<ContentResult>());
            var content = (ContentResult)result;
            Assert.That(content.Content, Does.Contain("Bienvenido"));
        }

        [Test]
        public void login_ReturnsContentResult_ForTeacher()
        {
            var user = new UserModel { Name = "Profesor", UserType = UserTypes.Teacher, Id = 2, Email = "b@b.com" };
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.login(It.IsAny<UserModel>())).Returns(user);
            var controller = GetControllerWithRepoMock(repo);
            var result = controller.login(new UserModel());
            Assert.That(result, Is.InstanceOf<ContentResult>());
            var content = (ContentResult)result;
            Assert.That(content.Content, Does.Contain("Bienvenido"));
        }

        [Test]
        public void login_RedirectsToLoginPage_OnNullUser()
        {
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.login(It.IsAny<UserModel>())).Returns((UserModel?)null);
            var controller = GetControllerWithRepoMock(repo);
            var result = controller.login(new UserModel());
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("LoginPage"));
        }

        [Test]
        public void login_RedirectsToLoginPage_OnUnknownUserType()
        {
            var user = new UserModel { Name = "Desconocido", UserType = (UserTypes)99, Id = 4, Email = "d@d.com" };
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.login(It.IsAny<UserModel>())).Returns(user);
            var controller = GetControllerWithRepoMock(repo);
            var result = controller.login(new UserModel());
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("LoginPage"));
        }

        [Test]
        public void registerUser_ReturnsView_OnSuccess()
        {
            var user = new UserModel { Name = "Nuevo", Id = 3, Email = "c@c.com" };
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.registerUser(It.IsAny<UserModel>())).Returns(10);
            var controller = GetControllerWithRepoMock(repo);
            var result = controller.registerUser(user);
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var view = (ViewResult)result;
            Assert.That(view.ViewName, Is.EqualTo("RedirectSuccesfulRegisterToAutoLogin"));
        }

        [Test]
        public void registerUser_RedirectsToLoginPage_OnFailure()
        {
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.registerUser(It.IsAny<UserModel>())).Returns(0);
            var controller = GetControllerWithRepoMock(repo);
            var result = controller.registerUser(new UserModel());
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("loginPage"));
        }
    }
}
