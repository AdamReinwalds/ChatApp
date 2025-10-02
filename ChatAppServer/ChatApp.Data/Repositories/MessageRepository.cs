using ChatApp.Data.Entities;
using ChatApp.Data.Interfaces;

namespace ChatApp.Data.Repositories;

public class MessageRepository(AppDbContext context) : BaseRepository<MessageEntity>(context), IMessageRepository
{
}

