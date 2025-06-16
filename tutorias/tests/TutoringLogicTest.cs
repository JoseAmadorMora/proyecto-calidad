using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using tutorias.Models;
using tutorias.Backend.Tutoring;

namespace NUnitTests
{
    public class TutoringLogicTest
    {
        private Mock<ITutoringRepository> repo;
        private TutoringLogic logic;

        [SetUp]
        public void Setup()
        {
            repo = new Mock<ITutoringRepository>();
            logic = new TutoringLogic(repo.Object);
        }

        [Test]
        public void GetAllTutorships_ReturnsList()
        {
            var expected = new List<TutoringModel> { new TutoringModel(), new TutoringModel() };
            repo.Setup(r => r.GetAllTutorships()).Returns(expected);

            var result = logic.GetAllTutorships();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetTutorshipById_ReturnsModel()
        {
            var tutorship = new TutoringModel { Id = 1 };
            repo.Setup(r => r.GetTutorshipById(1)).Returns(tutorship);

            var result = logic.GetTutorshipById(1);

            Assert.That(result, Is.EqualTo(tutorship));
        }

        [Test]
        public void SearchTutorships_ReturnsFilteredList()
        {
            var filtered = new List<TutoringModel> { new TutoringModel { CourseName = "Programacion I" } };
            repo.Setup(r => r.SearchTutorships("Programacion I", "Rodrigo Facio", null)).Returns(filtered);

            var result = logic.SearchTutorships("Programacion I", "Rodrigo Facio");

            Assert.That(result, Is.EqualTo(filtered));
        }

        [Test]
        public void AddTutorship_ReturnsTrue()
        {
            var tutorship = new TutoringModel();
            repo.Setup(r => r.AddTutorship(tutorship)).Returns(true);

            var result = logic.AddTutorship(tutorship);

            Assert.That(result, Is.True);
        }

        [Test]
        public void UpdateTutorship_ReturnsTrue()
        {
            var tutorship = new TutoringModel { Id = 1 };
            repo.Setup(r => r.UpdateTutorship(tutorship)).Returns(true);

            var result = logic.UpdateTutorship(tutorship);

            Assert.That(result, Is.True);
        }

        [Test]
        public void DeleteTutorship_ReturnsTrue()
        {
            repo.Setup(r => r.DeleteTutorship(1, 42)).Returns(true);

            var result = logic.DeleteTutorship(1, 42);

            Assert.That(result, Is.True);
        }

        [Test]
        public void GetTutorshipsByProfessorId_ReturnsList()
        {
            var list = new List<TutoringModel> { new TutoringModel { ProfessorId = 42 } };
            repo.Setup(r => r.GetTutorshipsByProfessorId(42)).Returns(list);

            var result = logic.GetTutorshipsByProfessorId(42);

            Assert.That(result, Is.EqualTo(list));
        }
    }
}