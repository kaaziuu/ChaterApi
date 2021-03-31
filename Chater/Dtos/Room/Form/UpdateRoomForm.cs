using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Chater.Dtos.Room.Form
{
    public class UpdateRoomForm
    {
        [HiddenInput]
        public string Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        [Required]
        public string NewName { get; set; }
        public string Password { get; set; }
    }
}