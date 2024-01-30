using System.ComponentModel.DataAnnotations;

namespace Pastebug.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public int CreatedPostsCount { get; set; }
}
