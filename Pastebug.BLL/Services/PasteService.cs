using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Pastebug.BLL.constants;
using Pastebug.BLL.Dtos;
using Pastebug.BLL.Exceptions;
using Pastebug.BLL.Specs;
using Pastebug.BLL.Utils;
using Pastebug.BLL.Vms;
using Pastebug.DAL.Repositories;
using Pastebug.Domain.Entities;

namespace Pastebug.BLL.Services;

public class PasteService : IPasteService
{
    private IPasteRepository _pasteRepository;
    private readonly IHashGenerator _hashGenerator;
    private readonly IPasteSpecifications _pasteSpecifications;
    private readonly IMapper _mapper;

    public PasteService(
        IPasteRepository pasteRepository,
        IHashGenerator hashGenerator,
        IPasteSpecifications pasteSpecifications,
        IMapper mapper)
    {
        _pasteRepository = pasteRepository;
        _hashGenerator = hashGenerator;
        _pasteSpecifications = pasteSpecifications;
        _mapper = mapper;
    }

    public async Task<PasteVm> GetPasteByHash(string hash)
    {
        Paste? paste = await _pasteRepository.FindAsync(hash);
        return _mapper.Map<PasteVm>(paste);
    }

    public async Task<string> CreatePaste(PasteDto pasteDto)
    {
        if (pasteDto == null)
        {
            throw new ApiException("Paste can't be null") { Code = 400 };
        }

        string hash = _hashGenerator.Generate();

        Paste pasteMapped = _mapper.Map<Paste>(pasteDto);
        pasteMapped.Hash = hash;
        pasteMapped.CreationDate = DateTime.Now;
        pasteMapped.PasteExposure = new PasteExposure
        {
            Id = Guid.NewGuid(),
            ExposureId = pasteDto.Visibility,
            PasteHash = hash,
            Paste = pasteMapped
        };

        await _pasteRepository.CreateAsync(pasteMapped);

        return hash;
    }

    public async Task<List<PasteVm>> SearchPaste(PasteFilterModel pasteFilter, Guid userId)
    {
        if (pasteFilter == null) return null;

        List<Paste> pastes =
            await _pasteRepository.SearchAsync(_pasteSpecifications.IsSatisfiesPasteFilter(pasteFilter));

        if (pasteFilter.UserId != userId)
        {
            pastes = pastes.Where(p => p.Visibility == (int)PasteVisibility.Public).ToList();
        }

        List<PasteVm> result = pastes.IsNullOrEmpty()
            ? new()
            : pastes
                .Select(p => _mapper.Map<PasteVm>(p))
                .ToList();

        return result;
    }

    public async Task RemovePaste(string hash, Guid userId)
    {
        Paste? paste = await _pasteRepository.FindAsync(hash);

        if (paste == null)
        {
            throw new ApiException("Paste to delete not found") { Code = 404 };
        }

        if (paste.UserId != userId)
        {
            throw new ApiException("Can't delete paste of other users") { Code = 403 };
        }

        await _pasteRepository.Delete(paste);
    }
}