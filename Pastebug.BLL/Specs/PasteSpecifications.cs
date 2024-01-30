using Microsoft.IdentityModel.Tokens;
using Pastebug.BLL.Dtos;
using Pastebug.Domain.Entities;
using System.Linq.Expressions;

namespace Pastebug.BLL.Specs;

public class PasteSpecifications : IPasteSpecifications
{
    public Expression<Func<Paste, bool>> IsSatisfiesPasteFilter(PasteFilterModel pasteFilter) => p => pasteFilter.UserId == p.UserId
                && (pasteFilter.Title.IsNullOrEmpty() ? true : p.Title.Contains(pasteFilter.Title!));
}
