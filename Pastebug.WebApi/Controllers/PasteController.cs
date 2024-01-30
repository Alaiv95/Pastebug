using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pastebug.BLL.Dtos;
using Pastebug.BLL.Services;
using Pastebug.BLL.Vms;
using Pastebug.WebApi.utils;

namespace Pastebug.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasteController : ControllerBase
    {
        private IPasteService _pasteService;
        private ICurrentUserChecker _checker;

        public PasteController(IPasteService pasteService, ICurrentUserChecker checker)
        {
            _pasteService = pasteService;
            _checker = checker;
        }

        [HttpGet("{hash}")]
        public async Task<IActionResult> GetPaste(string hash)
        {
            PasteVm paste = await _pasteService.GetPasteByHash(hash);
            return paste == null ? NotFound() : Ok(paste);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePaste([FromBody] PasteDto pasteDto)
        {
            Guid userId = await _checker.UserId();
            pasteDto.UserId = userId;

            string result = await _pasteService.CreatePaste(pasteDto);

            return Ok(result);
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchPaste([FromBody] PasteFilterModel pasteFilter)
        {
            Guid userId = await _checker.UserId();

            List<PasteVm> result = await _pasteService.SearchPaste(pasteFilter, userId);

            return result == null ? NoContent() : Ok(result);
        }

        [Authorize]
        [HttpDelete("remove/{hash}")]
        public async Task<IActionResult> RemovePaste([FromRoute] string hash)
        {
            Guid userId = await _checker.UserId();

            await _pasteService.RemovePaste(hash, userId);

            return NoContent();
        }
    }
}
