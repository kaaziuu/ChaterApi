using System.ComponentModel.DataAnnotations;

namespace Chater.Dtos.Room.Form
{
    public class AddUserFromRoom
    {
        [Required]
        public string UserId { get; set; }
        public string RoomPassword { get; set; }
        public int Role { get; set; }
    }
}