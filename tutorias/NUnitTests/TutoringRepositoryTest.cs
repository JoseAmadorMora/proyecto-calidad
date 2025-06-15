using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using tutorias.Backend.Tutoring;
using tutorias.Models;

namespace NUnitTests
{
    public class TutoringRepositoryTest
    {
        [Test]
        public void AddTutorship_ReturnsTrue()
        {
            var repo = new Mock<ITutoringRepository>();
            var model = new TutoringModel();
            repo.Setup(r => r.AddTutorship(model)).Returns(true);
            Assert.That(repo.Object.AddTutorship(model), Is.True);
        }

        [Test]
        public void UpdateTutorship_ReturnsTrue()
        {
            var repo = new Mock<ITutoringRepository>();
            var model = new TutoringModel();
            repo.Setup(r => r.UpdateTutorship(model)).Returns(true);
            Assert.That(repo.Object.UpdateTutorship(model), Is.True);
        }

        [Test]
        public void DeleteTutorship_ReturnsTrue()
        {
            var repo = new Mock<ITutoringRepository>();
            repo.Setup(r => r.DeleteTutorship(1, 2)).Returns(true);
            Assert.That(repo.Object.DeleteTutorship(1, 2), Is.True);
        }

        [Test]
        public void GetAllTutorships_ReturnsList()
        {
            var repo = new Mock<ITutoringRepository>();
            var list = new List<TutoringModel> { new TutoringModel() };
            repo.Setup(r => r.GetAllTutorships()).Returns(list);
            Assert.That(repo.Object.GetAllTutorships(), Is.EqualTo(list));
        }

        [Test]
        public void GetTutorshipById_ReturnsModel()
        {
            var repo = new Mock<ITutoringRepository>();
            var model = new TutoringModel { Id = 10 };
            repo.Setup(r => r.GetTutorshipById(10)).Returns(model);
            Assert.That(repo.Object.GetTutorshipById(10), Is.EqualTo(model));
        }

        [Test]
        public void SearchTutorships_ReturnsList()
        {
            var repo = new Mock<ITutoringRepository>();
            var list = new List<TutoringModel> { new TutoringModel() };
            repo.Setup(r => r.SearchTutorships("Programacion I", "Rodrigo Facio", null)).Returns(list);
            Assert.That(repo.Object.SearchTutorships("Programacion I", "Rodrigo Facio", null), Is.EqualTo(list));
        }

        [Test]
        public void GetTutorshipsByProfessorId_ReturnsList()
        {
            var repo = new Mock<ITutoringRepository>();
            var list = new List<TutoringModel> { new TutoringModel { ProfessorId = 7 } };
            repo.Setup(r => r.GetTutorshipsByProfessorId(7)).Returns(list);
            Assert.That(repo.Object.GetTutorshipsByProfessorId(7), Is.EqualTo(list));
        }
    }
}
