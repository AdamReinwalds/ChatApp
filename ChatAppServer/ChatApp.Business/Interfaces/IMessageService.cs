using ChatApp.Business.DTOs;
using ChatApp.Data.Entities;

namespace ChatApp.Business.Interfaces;

public interface IMessageService
{
    Task<MessageDto> CreateMessageAsync(int channelId, string userId, string text, string username);
    Task<IEnumerable<MessageDto>> GetMessagesByChannelIdAsync(int channelId, int take = 50);
}
