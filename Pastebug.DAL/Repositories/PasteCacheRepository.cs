using Microsoft.Extensions.Caching.Memory;
using Pastebug.Domain.Entities;
using System.Linq.Expressions;

namespace Pastebug.DAL.Repositories;

public class PasteCacheRepository : IPasteRepository
{
    PasteRepository _pasteRepository;
    IMemoryCache _memoryCache;

    public PasteCacheRepository(IMemoryCache memoryCache, PasteRepository pasteRepository)
    {
        _pasteRepository = pasteRepository;
        _memoryCache = memoryCache;
    }

    public async Task CreateAsync(Paste paste)
    {
        await _pasteRepository.CreateAsync(paste);
    }

    public async Task Delete(Paste paste)
    {
        var key = $"Paste-{paste.Hash}";
        _memoryCache.Remove(key);

        await _pasteRepository.Delete(paste);
    }

    public async Task<Paste?> FindAsync(string hash)
    {
        var key = $"Paste-{hash}";

        return await _memoryCache.GetOrCreateAsync(key, entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromHours(4));

            return _pasteRepository.FindAsync(hash);
        });
    }

    public async Task<List<Paste>> SearchAsync(Expression<Func<Paste, bool>> predicate)
    {
        return await _pasteRepository.SearchAsync(predicate);
    }
}
