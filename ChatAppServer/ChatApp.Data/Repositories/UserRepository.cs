using ChatApp.Data.Entities;
using ChatApp.Data.Interfaces;

namespace ChatApp.Data.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<UserEntity>(context), IUserRepository
{
}
