using System.ComponentModel.DataAnnotations;

namespace Chater.Dtos.User
{
    public class CreateUserDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Surname { get; set; }
    }
}