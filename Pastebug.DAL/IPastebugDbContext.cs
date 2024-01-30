using Microsoft.EntityFrameworkCore;
using Pastebug.Domain.Entities;

namespace Pastebug.DAL
{
    public interface IPastebugDbContext
    {
        DbSet<Paste> Pastes { get; set; }
        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken token);
    }
}