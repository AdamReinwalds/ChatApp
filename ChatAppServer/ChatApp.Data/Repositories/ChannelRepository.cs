using ChatApp.Data.Entities;
using ChatApp.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace ChatApp.Data.Repositories;

public class ChannelRepository(AppDbContext context, ILogger<BaseRepository<TextChannelEntity>> logger) : BaseRepository<TextChannelEntity>(context, logger), IChannelRepository
{
}
