using System.Data.SqlClient;
using Dapper;

namespace tutorias.Backend.Tutoring
{
    public interface ITutoringRepository
    {
        bool AddTutorship(TutoringModel tutorship);
        bool UpdateTutorship(TutoringModel tutorship);
        bool DeleteTutorship(int id, int professorId);
        List<TutoringModel> GetAllTutorships();
        TutoringModel? GetTutorshipById(int id);
        List<TutoringModel> SearchTutorships(string? course, string? sede, string? school);
        List<TutoringModel> GetTutorshipsByProfessorId(int professorId);
    }
    public class TutoringRepository : ITutoringRepository
    {
        private readonly SqlConnection sqlConnection;

        public TutoringRepository(SqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public bool AddTutorship(TutoringModel tutorship)
        {
            string sql = @"INSERT INTO Tutorship (Id, CourseInitials, CourseName, [Group], Sede, School, Semester, [Year], [Description], ProfessorId)
                          VALUES (@Id, @CourseInitials, @CourseName, @Group, @Sede, @School, @Semester, @Year, @Description, @ProfessorId)";
            return sqlConnection.Execute(sql, tutorship) > 0;
        }

        public bool DeleteTutorship(int id, int professorId)
        {
            string sql = "DELETE FROM Tutorship WHERE Id = @Id AND ProfessorId = @ProfessorId";
            return sqlConnection.Execute(sql, new { Id = id, ProfessorId = professorId }) > 0;
        }

        public List<TutoringModel> GetAllTutorships()
        {
            string sql = @"SELECT t.*, u.Name as ProfessorName
                          FROM Tutorship t
                          JOIN Users u ON t.ProfessorId = u.Id";
            return sqlConnection.Query<TutoringModel>(sql).ToList();
        }

        public TutoringModel? GetTutorshipById(int id)
        {
            string sql = @"SELECT t.*, u.Name as ProfessorName
                          FROM Tutorship t
                          JOIN Users u ON t.ProfessorId = u.Id
                          WHERE t.Id = @Id";
            return sqlConnection.QueryFirstOrDefault<TutoringModel>(sql, new { Id = id });
        }

        public List<TutoringModel> GetTutorshipsByProfessorId(int professorId)
        {
            string sql = @"SELECT t.*, u.Name as ProfessorName
                          FROM Tutorship t
                          JOIN Users u ON t.ProfessorId = u.Id
                          WHERE t.ProfessorId = @ProfessorId";
            return sqlConnection.Query<TutoringModel>(sql, new { ProfessorId = professorId }).ToList();
        }

        public List<TutoringModel> SearchTutorships(string? course, string? sede, string? school)
        {
            var sql = @"
            SELECT * FROM Tutorship
            WHERE (@Course IS NULL OR CourseName LIKE @Course)
            OR (@Sede IS NULL OR Sede LIKE @Sede)
            OR (@School IS NULL OR School LIKE @School)";
            return sqlConnection.Query<TutoringModel>(sql, new
            {
                Course = string.IsNullOrEmpty(course) ? null : "%" + course + "%",
                Sede = string.IsNullOrEmpty(sede) ? null : "%" + sede + "%",
                School = string.IsNullOrEmpty(school) ? null : "%" + school + "%"
            }).ToList();
        }

        public bool UpdateTutorship(TutoringModel tutorship)
        {
            string sql = @"UPDATE Tutorship SET
                            CourseInitials = @CourseInitials,
                            CourseName = @CourseName,
                            [Group] = @Group,
                            Sede = @Sede,
                            School = @School,
                            Semester = @Semester,
                            [Year] = @Year,
                            [Description] = @Description
                          WHERE Id = @Id AND ProfessorId = @ProfessorId";
            return sqlConnection.Execute(sql, tutorship) > 0;
        }
    }
}
