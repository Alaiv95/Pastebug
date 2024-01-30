using System.ComponentModel.DataAnnotations;

namespace Pastebug.BLL.Dtos;

public class PasteFilterModel
{
    [Required]
    public Guid UserId { get; set; }

    public string? Title { get; set; }
}
