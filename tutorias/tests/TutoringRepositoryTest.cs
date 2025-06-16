using NUnit.Framework;
using tutorias.Backend.Tutoring;
using tutorias.Models;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace NUnitTests
{
    public class TutoringRepositoryTest
    {
        private SqliteConnection _conn;
        private TutoringRepository _repo;

        [SetUp]
        public void Setup()
        {
            SQLitePCL.Batteries.Init();
            _conn = new SqliteConnection("Data Source=:memory:");
            _conn.Open();

            _conn.Execute("""
                CREATE TABLE Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT
                );
            """);

            _conn.Execute("""
                CREATE TABLE Tutorship (
                    Id INTEGER PRIMARY KEY,
                    CourseInitials TEXT,
                    CourseName TEXT,
                    [Group] INTEGER,
                    Sede TEXT,
                    School TEXT,
                    Semester INTEGER,
                    [Year] INTEGER,
                    [Description] TEXT,
                    ProfessorId INTEGER,
                    FOREIGN KEY (ProfessorId) REFERENCES Users(Id)
                );
            """);

            _conn.Execute("INSERT INTO Users (Name) VALUES ('Professor X');");
            _repo = new TutoringRepository(_conn);
        }

        [TearDown]
        public void TearDown()
        {
            _conn.Dispose();
        }

        [Test]
        public void AddTutorship_SuccessfullyInserts()
        {
            var tutorship = CreateTutorship();
            var result = _repo.AddTutorship(tutorship);
            Assert.IsTrue(result);
        }

        [Test]
        public void GetAllTutorships_ReturnsInserted()
        {
            _repo.AddTutorship(CreateTutorship());
            var results = _repo.GetAllTutorships();
            Assert.That(results, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetTutorshipById_ReturnsCorrectItem()
        {
            var tutorship = CreateTutorship();
            _repo.AddTutorship(tutorship);
            var result = _repo.GetTutorshipById(tutorship.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(tutorship.CourseName, result!.CourseName);
        }

        [Test]
        public void UpdateTutorship_ChangesData()
        {
            var tutorship = CreateTutorship();
            _repo.AddTutorship(tutorship);
            tutorship.CourseName = "Changed";
            var updated = _repo.UpdateTutorship(tutorship);
            var loaded = _repo.GetTutorshipById(tutorship.Id);

            Assert.IsTrue(updated);
            Assert.AreEqual("Changed", loaded!.CourseName);
        }

        [Test]
        public void DeleteTutorship_RemovesRow()
        {
            var tutorship = CreateTutorship();
            _repo.AddTutorship(tutorship);
            var deleted = _repo.DeleteTutorship(tutorship.Id, tutorship.ProfessorId);
            var after = _repo.GetTutorshipById(tutorship.Id);

            Assert.IsTrue(deleted);
            Assert.IsNull(after);
        }

        [Test]
        public void SearchTutorships_FiltersCorrectly()
        {
            _repo.AddTutorship(CreateTutorship(courseName: "Programacion I", sede: "Rodrigo Facio"));
            var results = _repo.SearchTutorships("Programacion I", null, null);
            Assert.That(results, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetTutorshipsByProfessorId_ReturnsCorrectSet()
        {
            _repo.AddTutorship(CreateTutorship(professorId: 1));
            var results = _repo.GetTutorshipsByProfessorId(1);
            Assert.That(results, Has.Count.EqualTo(1));
        }

        private TutoringModel CreateTutorship(
            int id = 1,
            string courseInitials = "CS101",
            string courseName = "Intro",
            int group = 1,
            string sede = "Central",
            string school = "ECCI",
            int semester = 1,
            int year = 2025,
            string description = "Curso",
            int professorId = 1)
        {
            return new TutoringModel
            {
                Id = id,
                CourseInitials = courseInitials,
                CourseName = courseName,
                Group = group,
                Sede = sede,
                School = school,
                Semester = semester,
                Year = year,
                Description = description,
                ProfessorId = professorId
            };
        }
    }
}
