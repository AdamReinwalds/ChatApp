using ChatApp.Data.Entities;
using ChatApp.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace ChatApp.Data.Repositories;

public class MessageRepository(AppDbContext context, ILogger<BaseRepository<MessageEntity>> logger) : BaseRepository<MessageEntity>(context, logger), IMessageRepository
{
}

