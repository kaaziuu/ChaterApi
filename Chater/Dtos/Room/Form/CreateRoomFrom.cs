using System.ComponentModel.DataAnnotations;

namespace Chater.Dtos.Room.Form
{
    public class CreateRoomForm
    {   
        [Required]
        public string Name { get; set; }
        public string Password { get; set; }
    }
}