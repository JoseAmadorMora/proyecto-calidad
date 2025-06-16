using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using System.Reflection;
using tutorias.Backend.Authentication;
using tutorias.Features.Authentication;
using tutorias.Models;

namespace NUnitTests
{
    public class AuthenticationControllerTest
    {
        AuthenticationController GetControllerWithRepoMock(Mock<IAuthenticationRepository> repoMock)
        {
            var logic = new AuthenticationLogic(repoMock.Object);
            var controller = new AuthenticationController(logic);
          
            var httpContext = new DefaultHttpContext();

            var sessionStorage = new Dictionary<string, byte[]>();

            var sessionMock = new Mock<ISession>();
            sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                       .Callback<string, byte[]>((key, value) => sessionStorage[key] = value);

            sessionMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                       .Returns((string key, out byte[] value) =>
                       {
                           var found = sessionStorage.TryGetValue(key, out var val);
                           value = val;
                           return found;
                       });

            httpContext.Session = sessionMock.Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

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
        public void login_ReturnsViewResult_ForStudent()
        {
            var user = new UserModel { Name = "Estudiante", UserType = UserTypes.Student, Id = 1, Email = "a@a.com" };
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.login(It.IsAny<UserModel>())).Returns(user);
            var controller = GetControllerWithRepoMock(repo);

            var result = controller.login(new UserModel());
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());

            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("TutoringMain"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Tutoring"));
        }

        [Test]
        public void login_ReturnsViewResult_ForTeacher()
        {
            var user = new UserModel { Name = "Profesor", UserType = UserTypes.Teacher, Id = 2, Email = "b@b.com" };
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.login(It.IsAny<UserModel>())).Returns(user);
            var controller = GetControllerWithRepoMock(repo);

            var result = controller.login(new UserModel());
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());

            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("TutoringMain"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Tutoring"));
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
            var repo = new Mock<IAuthenticationRepository>();            
            repo.Setup(r => r.login(It.IsAny<UserModel>())).Returns((UserModel ?)null);

            var controller = GetControllerWithRepoMock(repo);
            var result = controller.login(new UserModel());

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("LoginPage"));
        }

        [Test]
        public void login_SetsCorrectSessionValues()
        {
            var user = new UserModel { Id = 1, UserType = UserTypes.Student };
             var repo = new Mock<IAuthenticationRepository>();
             repo.Setup(r => r.login(It.IsAny<UserModel>())).Returns(user);
             var controller = GetControllerWithRepoMock(repo);

             var result = controller.login(new UserModel());

             Assert.That(controller.HttpContext.Session.GetInt32("UserId"), Is.EqualTo(1));
             Assert.That(controller.HttpContext.Session.GetInt32("UserType"), Is.EqualTo((int)UserTypes.Student));            
        }

        [Test]
        public void login_SetsTempData_OnFailure()
        {
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.login(It.IsAny<UserModel>())).Returns((UserModel?)null);
            var controller = GetControllerWithRepoMock(repo);

            var result = controller.login(new UserModel());

            Assert.That(controller.TempData["LoginError"], Is.EqualTo("usuario o contrasena incorrectos o inexistentes"));
        }

        [Test]
        public void registerUser_SetsTempData_OnFailure()
        {
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.registerUser(It.IsAny<UserModel>())).Returns(0);
            var controller = GetControllerWithRepoMock(repo);

            var result = controller.registerUser(new UserModel());

            Assert.That(controller.TempData["RegisterError"], Is.EqualTo("No se pudo registrar el usuario"));
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
        public void registerUser_NotRedirectsToLoginPage_OnFailure()
        {
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.registerUser(It.IsAny<UserModel>())).Returns(0);
            var controller = GetControllerWithRepoMock(repo);
            var result = controller.registerUser(new UserModel());
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("RegisterPage"));
        }

        [Test]
        public void registerUser_RedirectsToRegisterPage_OnDuplicateEmailException()
        {
            var user = new UserModel { Email = "dup@mail.com" };
            var repo = new Mock<IAuthenticationRepository>();
            repo.Setup(r => r.registerUser(It.IsAny<UserModel>()))
                .Throws(new InvalidOperationException($"El correo {user.Email} ya est� asociado a un usuario en el sistema, debe usar otro"));
            var controller = GetControllerWithRepoMock(repo);
            var result = controller.registerUser(user);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("RegisterPage"));
            Assert.That(controller.TempData["RegisterError"]?.ToString(), Does.Contain(user.Email));
        }
    }

    
}