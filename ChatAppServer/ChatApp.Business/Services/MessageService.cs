using ChatApp.Business.DTOs;
using ChatApp.Business.Helpers;
using ChatApp.Business.Interfaces;
using ChatApp.Data.Entities;
using ChatApp.Data.Interfaces;

namespace ChatApp.Business.Services;

public class MessageService(IMessageRepository messageRepository, EncryptionHelper encryptionHelper) : IMessageService
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly EncryptionHelper _encryptionHelper = encryptionHelper;
    public async Task<MessageDto> CreateMessageAsync(int channelId, string userId, string text, string username)
    {
        int parsedUserId = int.Parse(userId);
        var encryptedText = _encryptionHelper.Encrypt(text);
        var message = new MessageEntity
        {
            ChannelId = channelId,
            Content = encryptedText,
            CreatedAt = DateTime.UtcNow,
            UserId = parsedUserId
        };


        var result = await _messageRepository.AddAsync(message);
        if (!result) return null!;

        return new MessageDto
        {
            Id = message.Id,
            Username = username,
            Text = text,
            CreatedAt = message.CreatedAt
        };
    }


    //TODO: Optimize so it doesn't fetch user for every message
    public async Task<IEnumerable<MessageDto>> GetMessagesByChannelIdAsync(int channelId, int take = 50)
    {
        var messages =  await _messageRepository.GetAllAsync(
            filterBy: m => m.ChannelId == channelId,
            orderByDescending: false,
            sortBy: m => m.CreatedAt,
            take: take,
            includes: m => m.User
        );
     

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            Username = m.User.Username,
            Text = _encryptionHelper.Decrypt(m.Content),
            CreatedAt = m.CreatedAt
        });
    }
}
