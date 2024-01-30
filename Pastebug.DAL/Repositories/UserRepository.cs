using Microsoft.EntityFrameworkCore;
using Pastebug.Domain.Entities;

namespace Pastebug.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private IPastebugDbContext _pastebugDbContext;

    public UserRepository(IPastebugDbContext pastebugDbContext) => _pastebugDbContext = pastebugDbContext;

    public async Task Register(User user)
    {
        await _pastebugDbContext.Users.AddAsync(user);
        await _pastebugDbContext.SaveChangesAsync(CancellationToken.None);
    }

    public async Task<User> FinByEmailAsync(string email)
    {
        return await _pastebugDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
