using System.ComponentModel.DataAnnotations;

namespace Chater.Dtos.User.From
{
    public record CreateUserDto
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