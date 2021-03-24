using System.ComponentModel.DataAnnotations;

namespace Chater.Dtos.Room.From
{
    public class CreateUpdateRoomDto
    {   
        [Required]
        public string Name { get; set; }
        public string Password { get; set; }
    }
}