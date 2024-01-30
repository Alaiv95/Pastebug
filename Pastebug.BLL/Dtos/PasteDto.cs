using AutoMapper;
using Pastebug.BLL.Mapping;
using Pastebug.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pastebug.BLL.Dtos;

public class PasteDto : IMapWith<Paste>
{
    public string Title { get; set; }

    [Required]
    public string Text { get; set; }

    public string Syntax { get; set; }

    [Range(1, 2, ErrorMessage = "Invalid type. Choose 1 - public, 2 - private")]
    public int Visibility { get; set; }

    public DateTime ExpireDate { get; set; }

    [JsonIgnore]
    public Guid UserId { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PasteDto, Paste>();
    }
}
