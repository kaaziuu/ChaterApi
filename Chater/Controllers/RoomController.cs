using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Dtos.Room.From;
using Chater.Dtos.Room.Response;
using Chater.Dtos.User.Response;
using Chater.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Chater.Controllers
{
    [ApiController]
    [Authorize]
    [Route("room")]
    public class RoomController
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
            throw new System.NotImplementedException();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<RoomDto>> GetRoomAsync(string id)
        {
            throw new System.NotImplementedException();
        }


        [HttpPost]
        public async Task<ActionResult<RoomDto>> CreateRoomAsync(CreateUpdateRoomDto request)
        {
            throw new System.NotImplementedException();
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
        public async Task<ActionResult<RoomAction>> AddUserToRoomAsync(string id, UserActionAtRoomDto user)
        {
            throw new System.NotImplementedException();
        }

        [HttpDelete]
        [Route("{id}/user")]
        public async Task<ActionResult<RoomAction>> RemoveUserFromRoomAsync(string id, UserActionAtRoomDto user)
        {
            throw new System.NotImplementedException();
        }




    }
}