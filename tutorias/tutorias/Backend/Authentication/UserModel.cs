using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public enum UserTypes
    {
        Student = 0,
        Teacher = 1
    }

    public class UserModel
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string? Name { get; set; }        
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(255)]
        public string? Password { get; set; }
        public UserTypes UserType { get; set; }
    }
}
