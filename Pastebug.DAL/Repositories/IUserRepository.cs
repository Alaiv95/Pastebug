using Pastebug.Domain.Entities;

namespace Pastebug.DAL.Repositories;

public interface IUserRepository
{
    Task Register(User user);
    Task<User> FinByEmailAsync(string email);
}
