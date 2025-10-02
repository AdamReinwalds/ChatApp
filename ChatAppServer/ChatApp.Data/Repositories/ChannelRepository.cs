using ChatApp.Data.Entities;
using ChatApp.Data.Interfaces;

namespace ChatApp.Data.Repositories;

public class ChannelRepository(AppDbContext context) : BaseRepository<TextChannelEntity>(context), IChannelRepository
{
}
