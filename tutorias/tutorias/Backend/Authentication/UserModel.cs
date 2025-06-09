using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string? Name { get; set; }        
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(255)]
        public string? Password { get; set; }
        public short UserType { get; set; } // 0: Student, 1: Teacher
    }
}
