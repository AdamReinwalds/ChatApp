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
public class ChannelController(IChannelService channelService) : Controller
{
    private readonly IChannelService _channelService = channelService;
    public record CreateChannelRequest(string Name, string? Description, bool IsPrivate);

    [HttpPost("create")]
    public async Task<IActionResult> CreateChannel(CreateChannelRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Channel name is required");

        if (userIdClaim == null)
            return Unauthorized("User ID claim is missing");

        var channelDto = new CreateChannelDto
        {
            Name = request.Name,
            Description = request.Description,
            IsPrivate = request.IsPrivate,
            CreatedByUserId = int.Parse(userIdClaim)
        };

        var result = await _channelService.CreateChannelAsync(channelDto);

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
            return BadRequest("Search term is required");
        var channels = await _channelService.GetPublicChannelsByNameSearch(name);
        return Ok(channels);
    }

    [HttpPost("join/{id}")]
    public async Task<IActionResult> JoinChannel(string id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("User ID claim is missing");
        var result = await _channelService.JoinChannelAsync(int.Parse(id), int.Parse(userIdClaim));
        if (!result.Success)
            return BadRequest(result.Message);
        return Ok(result);
    }
}
