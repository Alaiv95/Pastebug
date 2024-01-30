using Pastebug.BLL.Dtos;
using Pastebug.BLL.Vms;

namespace Pastebug.BLL.Services;

public interface IPasteService
{
    Task<PasteVm> GetPasteByHash(string hash);
    Task<string> CreatePaste(PasteDto paste);
    Task<List<PasteVm>> SearchPaste(PasteFilterModel pasteFilter, Guid userId);
    Task RemovePaste(string hash, Guid userId);
}