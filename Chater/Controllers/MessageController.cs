using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Chater.Dtos.Message.Form;
using Chater.Repository.Abstract;
using Chater.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Chater.Controllers
{
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
        public async Task<AcceptedResult> NewMessage(string roomId, NewMessageForm form)
        {
            throw new NotImplementedException();
        }
        
    }
}