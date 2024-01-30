using System.ComponentModel.DataAnnotations;

namespace Pastebug.BLL.Dtos;

public class UserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
