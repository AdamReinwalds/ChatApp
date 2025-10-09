using ChatApp.Data.Entities;
using ChatApp.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace ChatApp.Data.Repositories;

public class UserRepository(AppDbContext context, ILogger<BaseRepository<UserEntity>> logger) : BaseRepository<UserEntity>(context, logger), IUserRepository
{
}
