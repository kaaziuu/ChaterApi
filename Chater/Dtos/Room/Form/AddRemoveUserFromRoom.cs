using System.ComponentModel.DataAnnotations;

namespace Chater.Dtos.Room.Form
{
    public class AddRemoveUserFromRoom
    {
        [Required]
        public string UserId { get; set; }
        public string RoomPassword { get; set; }
        [Required]
        public int Role { get; set; }
    }
}