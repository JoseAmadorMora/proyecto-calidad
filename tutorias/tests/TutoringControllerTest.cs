using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using tutorias.Backend.Authentication;
using tutorias.Backend.Tutorias;
using tutorias.Backend.Tutoring;
using tutorias.Models;

namespace NUnitTests
{
    public class TutoringControllerTest
    {
        private TutoringController GetControllerWithMock(Moq.Mock<ITutoringRepository> repoMock)
        {
            var logic = new TutoringLogic(repoMock.Object);
            var controller = new TutoringController(logic);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockHttpSession();
            httpContext.Session.SetInt32("UserId", 1);
            httpContext.Session.SetInt32("UserType", (int)UserTypes.Teacher);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        private (TutoringController controller, ViewResult result)
            GetControllerAndResultWithMock(Mock<ITutoringRepository> repoMock)
        {
            var logic = new TutoringLogic(repoMock.Object);
            var controller = new TutoringController(logic);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockHttpSession();
            httpContext.Session.SetInt32("UserId", 1);
            httpContext.Session.SetInt32("UserType", (int)UserTypes.Teacher);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = controller.TutoringMain() as ViewResult;
            return (controller, result);
        }

        [Test]
        public void TutoringMain_SetsViewBagValues()
        {
            var mockRepo = new Mock<ITutoringRepository>();
            mockRepo.Setup(r => r.GetAllTutorships()).Returns(new List<TutoringModel>());

            var (controller, result) = GetControllerAndResultWithMock(mockRepo);
            
            Assert.That(controller.ViewBag.UserId, Is.EqualTo(1));
            Assert.That(controller.ViewBag.UserType, Is.EqualTo((int)UserTypes.Teacher));
            Assert.That(controller.ViewBag.IsProfessor, Is.True);
        }


        [Test]
        public void AddTutorship_ReturnsUnauthorized_WhenNoSession()
        {
            var mockRepo = new Mock<ITutoringRepository>();
            var controller = GetControllerWithMock(mockRepo);
            controller.HttpContext.Session.Remove("UserId");

            var result = controller.AddTutorship(new TutoringModel());

            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public void TutoringMain_ReturnsViewResult_WithModel()
        {
            var mockRepo = new Mock<ITutoringRepository>();
            mockRepo.Setup(r => r.GetAllTutorships()).Returns(new List<TutoringModel>());
            var controller = GetControllerWithMock(mockRepo);

            var result = controller.TutoringMain();
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public void SearchTutorships_ReturnsFilteredTutorships()
        {
            var mockRepo = new Mock<ITutoringRepository>();
            mockRepo.Setup(r => r.SearchTutorships(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<TutoringModel> { new TutoringModel { CourseName = "Programacion I" } });

            var controller = GetControllerWithMock(mockRepo);
            var result = controller.SearchTutorships("Programacion I", null, null) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("TutoringMain"));
            Assert.That(result.Model, Is.InstanceOf<List<TutoringModel>>());
        }

        [Test]
        public void AddTutorship_Get_ReturnsViewWithEmptyModel()
        {
            var controller = GetControllerWithMock(new Mock<ITutoringRepository>());
            var result = controller.AddTutorship() as ViewResult;

            Assert.That(result.ViewName, Is.EqualTo("TutorshipForm"));
            Assert.That(result.Model, Is.InstanceOf<TutoringModel>());
        }

        [Test]
        public void AddTutorship_Post_Success_ReturnsRedirect()
        {
            var mockRepo = new Mock<ITutoringRepository>();
            mockRepo.Setup(r => r.AddTutorship(It.IsAny<TutoringModel>())).Returns(true);

            var controller = GetControllerWithMock(mockRepo);
            var result = controller.AddTutorship(new TutoringModel());

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("TutoringMain"));
        }

        [Test]
        public void AddTutorship_Post_Failure_ReturnsBadRequest()
        {
            var mockRepo = new Mock<ITutoringRepository>();
            mockRepo.Setup(r => r.AddTutorship(It.IsAny<TutoringModel>())).Returns(false);

            var controller = GetControllerWithMock(mockRepo);
            var result = controller.AddTutorship(new TutoringModel());

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void EditTutorship_Get_ReturnsViewWithModel()
        {
            var model = new TutoringModel { Id = 5 };
            var mockRepo = new Mock<ITutoringRepository>();
            mockRepo.Setup(r => r.GetTutorshipById(5)).Returns(model);

            var controller = GetControllerWithMock(mockRepo);
            var result = controller.EditTutorship(5) as ViewResult;

            Assert.That(result.ViewName, Is.EqualTo("TutorshipForm"));
            Assert.That(result.Model, Is.EqualTo(model));
        }

        [Test]
        public void EditTutorship_Get_NotFound()
        {
            var mockRepo = new Mock<ITutoringRepository>();
            mockRepo.Setup(r => r.GetTutorshipById(It.IsAny<int>())).Returns((TutoringModel?)null);

            var controller = GetControllerWithMock(mockRepo);
            var result = controller.EditTutorship(5);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void EditTutorship_Post_Success_ReturnsRedirect()
        {
            var tutorship = new TutoringModel { Id = 2, ProfessorId = 1 };
            var mockRepo = new Mock<ITutoringRepository>();
            mockRepo.Setup(r => r.UpdateTutorship(It.IsAny<TutoringModel>())).Returns(true);

            var controller = GetControllerWithMock(mockRepo);
            var result = controller.EditTutorship(tutorship);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("TutoringMain"));
        }

        [Test]
        public void EditTutorship_Post_Forbidden_WrongProfessor()
        {
            var tutorship = new TutoringModel { Id = 2, ProfessorId = 99 };
            var controller = GetControllerWithMock(new Mock<ITutoringRepository>());
            var result = controller.EditTutorship(tutorship);

            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public void DeleteTutorship_Success_Redirects()
        {
            var model = new TutoringModel { Id = 4, ProfessorId = 1 };
            var mockRepo = new Mock<ITutoringRepository>();
            mockRepo.Setup(r => r.GetTutorshipById(4)).Returns(model);
            mockRepo.Setup(r => r.DeleteTutorship(4, 1)).Returns(true);

            var controller = GetControllerWithMock(mockRepo);
            var result = controller.DeleteTutorship(4, 1);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public void DeleteTutorship_Forbidden_OnWrongProfessor()
        {
            var model = new TutoringModel { Id = 4, ProfessorId = 99 };
            var mockRepo = new Mock<ITutoringRepository>();
            mockRepo.Setup(r => r.GetTutorshipById(4)).Returns(model);

            var controller = GetControllerWithMock(mockRepo);
            var result = controller.DeleteTutorship(4, 99);

            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public void DeleteTutorship_Failure_ReturnsBadRequest()
        {
            var model = new TutoringModel { Id = 4, ProfessorId = 1 };
            var mockRepo = new Mock<ITutoringRepository>();
            mockRepo.Setup(r => r.GetTutorshipById(4)).Returns(model);
            mockRepo.Setup(r => r.DeleteTutorship(4, 1)).Returns(false);

            var controller = GetControllerWithMock(mockRepo);
            var result = controller.DeleteTutorship(4, 1);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void DeleteTutorship_HandlesNonExistentTutorship()
        {
            var mockRepo = new Mock<ITutoringRepository>();
            mockRepo.Setup(r => r.GetTutorshipById(It.IsAny<int>())).Returns((TutoringModel?)null);

            var controller = GetControllerWithMock(mockRepo);
            var result = controller.DeleteTutorship(999, 1);

            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }
    }

    public class MockHttpSession : ISession
    {
        private Dictionary<string, byte[]> _sessionStorage = new();

        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public string Id => Guid.NewGuid().ToString();

        public bool IsAvailable => true;

        public void Clear() => _sessionStorage.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _sessionStorage.Remove(key);

        public void Set(string key, byte[] value) => _sessionStorage[key] = value;

        public bool TryGetValue(string key, out byte[]? value) => _sessionStorage.TryGetValue(key, out value);

        public void SetInt32(string key, int value) => Set(key, BitConverter.GetBytes(value));

        public int? GetInt32(string key)
        {
            if (TryGetValue(key, out var data))
                return BitConverter.ToInt32(data, 0);
            return null;
        }
    }
}
