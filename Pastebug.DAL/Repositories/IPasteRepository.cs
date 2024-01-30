using Pastebug.Domain.Entities;
using System.Linq.Expressions;

namespace Pastebug.DAL.Repositories;

public interface IPasteRepository
{
    Task<Paste?> FindAsync(string hash);
    Task<List<Paste>> SearchAsync(Expression<Func<Paste, bool>> predicate);
    Task CreateAsync(Paste paste);
    Task Delete(Paste paste);
}