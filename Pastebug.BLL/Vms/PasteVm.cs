using AutoMapper;
using Pastebug.BLL.Mapping;
using Pastebug.Domain.Entities;

namespace Pastebug.BLL.Vms;

public class PasteVm : IMapWith<Paste>
{
    public string Title { get; set; }

    public string Text { get; set; }

    public string Syntax { get; set; }

    public int Visibility { get; set; }

    public DateTime ExpireDate { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Paste, PasteVm>();
    }
}
