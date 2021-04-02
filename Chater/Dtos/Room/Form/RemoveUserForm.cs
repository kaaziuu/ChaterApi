using System.ComponentModel.DataAnnotations;

namespace Chater.Dtos.Room.Form
{
    public class RemoveUserForm
    {
        [Required]
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}