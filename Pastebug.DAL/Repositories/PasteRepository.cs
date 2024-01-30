using Microsoft.EntityFrameworkCore;
using Pastebug.Domain.Entities;
using System.Linq.Expressions;

namespace Pastebug.DAL.Repositories;

public class PasteRepository : IPasteRepository
{
    private IPastebugDbContext _pastebugDbContext;

    public PasteRepository(IPastebugDbContext pastebugDbContext) => _pastebugDbContext = pastebugDbContext;

    public async Task CreateAsync(Paste paste)
    {
        await _pastebugDbContext.Pastes.AddAsync(paste);
        await _pastebugDbContext.SaveChangesAsync(CancellationToken.None);
    }

    public async Task Delete(Paste paste)
    {
       _pastebugDbContext.Pastes.Remove(paste);
       await _pastebugDbContext.SaveChangesAsync(default);
    }

    public async Task<Paste?> FindAsync(string hash)
    {
      return await _pastebugDbContext.Pastes
            .FirstOrDefaultAsync(p => p.Hash == hash);
    }

    public Task<List<Paste>> SearchAsync(Expression<Func<Paste, bool>> predicate)
    {
        var result = _pastebugDbContext.Pastes
            .Where(predicate)
            .ToListAsync();

        return result;
    }
}
