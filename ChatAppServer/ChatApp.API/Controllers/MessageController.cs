using ChatApp.Business.Interfaces;
using ChatApp.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]

[Authorize]
public class MessageController(IMessageService messageService) : Controller
{
    private readonly IMessageService _messageService = messageService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMessages(int id)
    {
        var messages = await _messageService.GetMessagesByChannelIdAsync(id);
        
        return Ok(messages);
    }
}
