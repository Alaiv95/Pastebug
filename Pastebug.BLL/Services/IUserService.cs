using Pastebug.BLL.Dtos;

namespace Pastebug.BLL.Services;

public interface IUserService
{
    Task<Guid> AddUserAsync(UserDto userDto, CancellationToken cancellationToken = default);
    Task<string> SignInAsync(UserDto userDto, CancellationToken cancellationToken = default);
    Task<Guid> GetUserIdAsync(string email, CancellationToken cancellationToken = default);
}