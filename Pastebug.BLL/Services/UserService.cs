using Pastebug.BLL.auth;
using Pastebug.BLL.Dtos;
using Pastebug.BLL.Exceptions;
using Pastebug.DAL.Repositories;
using Pastebug.Domain.Entities;

namespace Pastebug.BLL.Services;

public class UserService : IUserService
{
    private IUserRepository _userRepository;
    private IJwtToken _jwtToken;

    public UserService(IUserRepository userRepository, IJwtToken jwtToken)
    {
        _userRepository = userRepository;
        _jwtToken = jwtToken;
    }

    public async Task<Guid> AddUserAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        User user = new User
        {
            Id = Guid.NewGuid(),
            Email = userDto.Email,
            CreatedPostsCount = 0
        };

        await _userRepository.Register(user);

        return user.Id;
    }

    public async Task<Guid> GetUserIdAsync(string email, CancellationToken cancellationToken = default)
    {
        User user = await _userRepository.FinByEmailAsync(email);

        return user == null ? Guid.Empty : user.Id;
    }

    public async Task<string> SignInAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        User user = await _userRepository.FinByEmailAsync(userDto.Email);

        if (user == null)
        {
            throw new ApiException("User with correspoing email not found") { Code = 404 };
        }

        string token = _jwtToken.Generate(user);

        return token;
    }
}
