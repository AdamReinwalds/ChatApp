using ChatApp.Business.Interfaces;
using ChatApp.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]

[Authorize]
public class MessageController(IMessageService messageService, ILogger<MessageController> logger) : Controller
{
    private readonly IMessageService _messageService = messageService;
    private readonly ILogger<MessageController> _logger = logger;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMessages(int id)
    {
        try
        {
            var messages = await _messageService.GetMessagesByChannelIdAsync(id);
            return Ok(messages);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error fetching messages for channel ID {ChannelId}", id);
            throw;
        }
    }
}
