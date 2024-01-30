using Pastebug.BLL.Dtos;
using Pastebug.Domain.Entities;
using System.Linq.Expressions;

namespace Pastebug.BLL.Specs;

public interface IPasteSpecifications
{
    public Expression<Func<Paste, bool>> IsSatisfiesPasteFilter(PasteFilterModel pasteFilter);
}
