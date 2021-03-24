using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Chater.Dtos.Room.From;
using Chater.Dtos.Room.Response;
using Chater.Dtos.User.Response;
using Chater.Models;
using Chater.Service.Abstract;
using Chater.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Chater.Controllers
{
    [ApiController]
    [Authorize]
    [Route("room")]
    public class RoomController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IUserService _userService;
        private readonly IRoomService _roomService;

        public RoomController(IIdentityService identityService, IUserService userService, IRoomService roomService)
        {
            _identityService = identityService;
            _userService = userService;
            _roomService = roomService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsAsync()
        {
            User user = await _identityService.GetCurrentUserAsync(this.User.Identity as ClaimsIdentity);
            IEnumerable<RoomDto> rooms = await _userService.GetUserRoomsAsync(user);

            return Ok(rooms);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<RoomDto>> GetRoomAsync(string id)
        {
            throw new System.NotImplementedException();
        }


        [HttpPost]
        public async Task<ActionResult<RoomAction>> CreateRoomAsync(CreateUpdateRoomDto request)
        {
            User user = await _identityService.GetCurrentUserAsync(this.User.Identity as ClaimsIdentity);
            var result = await _roomService.CreateRoomAsync(request, user);
            if (!result.IsSuccessfully )
            {
                return this.BadRequest(result);
            }

            return Ok(result);

        }


        [HttpPut]
        public async Task<ActionResult> UpdateRoomAsync(CreateUpdateRoomDto request)
        {
            throw new System.NotImplementedException();
        }
        
        
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteRoomAsync(string id)
        {
            throw new System.NotImplementedException();
        }
        
        
        [HttpPost]
        [Route("{id}/user")]
        public async Task<ActionResult<RoomAction>> AddUserToRoomAsync(string id, AddRemoverFromRoom user)
        {
            throw new System.NotImplementedException();
        }

        [HttpDelete]
        [Route("{id}/user")]
        public async Task<ActionResult<RoomAction>> RemoveUserFromRoomAsync(string id, AddRemoverFromRoom user)
        {
            throw new System.NotImplementedException();
        }




    }
}