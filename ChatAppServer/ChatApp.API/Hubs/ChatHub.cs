using ChatApp.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.API.Hubs;

public class ChatHub(IMessageService messageService, IChannelService channelService) : Hub
{
    private readonly IMessageService _messageService = messageService;
    private readonly IChannelService _channelService = channelService;

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return;

        var userChannels = await _channelService.GetChannelListDtosAsync(int.Parse(userId));
        foreach (var channel in userChannels)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channel.Id.ToString());
        }
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(int channelId, string message)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (userId == null || username == null) return;
        
        var messageDto = await _messageService.CreateMessageAsync(channelId, userId, message, username);

        await Clients.Group(channelId.ToString())
            .SendAsync("ReceiveMessage", messageDto);

    }

    public async Task JoinChannel(int channelId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, channelId.ToString());
    }
}
