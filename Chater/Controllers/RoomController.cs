using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Chater.Dtos.Room;
using Chater.Dtos.Room.Form;
using Chater.Dtos.Room.Response;
using Chater.Dtos.User.Response;
using Chater.Exception;
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
        public async Task<ActionResult<RoomAction>> CreateRoomAsync(CreateRoomForm request)
        {
            User user = await _identityService.GetCurrentUserAsync(this.User.Identity as ClaimsIdentity);
            RoomAction result = null;
            try
            {
                result = await _roomService.CreateRoomAsync(request, user);
               
            }
            catch(System.Exception e)
            {
                return BadRequest(new RoomAction()
                {
                    Error = e.Message,
                    IsSuccessfully = false
                });
            }
            return Ok(result);

        }


        [HttpPut("{id}")]
        public async Task<ActionResult<RoomAction>> UpdateRoomAsync(string id,UpdateRoomForm request)
        {
            User user = await _identityService.GetCurrentUserAsync(this.User.Identity as ClaimsIdentity);
            try
            {
                request.Id = id;
                var roomAction = await _roomService.UpdateRoomAsync(request, user);
                return Ok(roomAction);
            }
            catch (System.Exception e)
            {
                return BadRequest(new RoomAction()
                {
                    Error = e.Message,
                    IsSuccessfully = false
                });
            }

        }
        
        
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteRoomAsync(string id)
        {
            throw new System.NotImplementedException();
        }
        
        
        [HttpPost]
        [Route("{id}/user")]
        public async Task<ActionResult> AddUserToRoomAsync(string id, AddRemoveUserFromRoom form)
        {
            try
            {
                await _roomService.GetRoomAndAddUserAsync(form, id);
                return Ok();
            }
            catch (System.Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpDelete]
        [Route("{id}/user")]
        public async Task<ActionResult> RemoveUserFromRoomAsync(string id, AddRemoveUserFromRoom form)
        {
            throw new System.NotImplementedException();
        }




    }
}