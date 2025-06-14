using System.ComponentModel.DataAnnotations;

namespace tutorias.Backend.Tutoring
{
    public class TutoringModel
    {
        public int Id { get; set; }

        [StringLength(30)]
        public string CourseInitials { get; set; } = string.Empty;

        [StringLength(50)]
        public string CourseName { get; set; } = string.Empty;

        public int Group { get; set; }

        [StringLength(50)]
        public string Sede { get; set; } = string.Empty;

        [StringLength(50)]
        public string School { get; set; } = string.Empty;

        public int Semester { get; set; }

        public int Year { get; set; }

        [StringLength(255)]
        public string Description { get; set; } = string.Empty;

        public int ProfessorId { get; set; }

        public string? ProfessorName { get; set; } // Obtained via JOIN

    }
}
