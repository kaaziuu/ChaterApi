using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Chater.Dtos.Message.Form;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chater.Controllers
{
    [ApiController]
    [Authorize]
    [Route("message")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IIdentityService _identityService;

        public MessageController(IMessageService messageService, IIdentityService identityService)
        {
            _messageService = messageService;
            _identityService = identityService;
        }
        
        [HttpPost("room/{id}")]
        public async Task<ActionResult> NewMessage(string id, NewMessageForm form)
        {
            try
            {
                User user = await _identityService.GetCurrentUserAsync(this.User.Identity as ClaimsIdentity);
                await _messageService.NewMessage(form, user, id);
                return Ok();
            }
            catch(System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
    }
}