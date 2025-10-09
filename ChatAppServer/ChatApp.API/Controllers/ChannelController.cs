using ChatApp.Business.DTOs;
using ChatApp.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChannelController(IChannelService channelService, ILogger<ChannelController> logger) : Controller
{
    private readonly IChannelService _channelService = channelService;
    private readonly ILogger<ChannelController> _logger = logger;
    public record CreateChannelRequest(string Name, string? Description, bool IsPrivate);

    [HttpPost("create")]
    public async Task<IActionResult> CreateChannel(CreateChannelRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            _logger.LogWarning("User {UserId} attempted to create a channel without a name", userIdClaim);
            return BadRequest("Channel name is required");
        }

        if (userIdClaim == null)
        {
            _logger.LogWarning("Unauthorized channel creation attempt (missing user ID claim)");
            return Unauthorized("User ID claim is missing");
        }

        var channelDto = new CreateChannelDto
        {
            Name = request.Name,
            Description = request.Description,
            IsPrivate = request.IsPrivate,
            CreatedByUserId = int.Parse(userIdClaim)
        };

        var result = await _channelService.CreateChannelAsync(channelDto);

        if (!result.Success)
        {
            _logger.LogWarning("Failed to create channel {ChannelName} for user {UserId}: {Message}", request.Name, userIdClaim, result.Message);
            return BadRequest(result.Message);
        }
        return Ok(result);
    }
    [HttpGet("my")]
    public async Task<IActionResult> GetMyChannels()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var channels = await _channelService.GetChannelListDtosAsync(int.Parse(userIdClaim!));
        return Ok(channels);
    }

    [HttpGet("public")]
    public async Task<IActionResult> GetPublicChannels()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var channels = await _channelService.GetPublicChannelListDtosAsync(int.Parse(userIdClaim!));
        return Ok(channels);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchPublicChannels([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogWarning("Invalid channel search — empty name parameter");
            return BadRequest("Search term is required");
        }
        var channels = await _channelService.GetPublicChannelsByNameSearch(name);
        return Ok(channels);
    }

    [HttpPost("join/{id}")]
    public async Task<IActionResult> JoinChannel(string id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            _logger.LogWarning("Unauthorized join attempt for channel {ChannelId} (missing user ID claim)", id);
            return Unauthorized("User ID claim is missing");
        }
        var result = await _channelService.JoinChannelAsync(int.Parse(id), int.Parse(userIdClaim));
        if (!result.Success)
        {
            _logger.LogWarning("User {UserId} failed to join channel {ChannelId}: {Message}", userIdClaim, id, result.Message);
            return BadRequest(result.Message);
        }
        return Ok(result);
    }
}
