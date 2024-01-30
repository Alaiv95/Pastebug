using Pastebug.BLL.Services;
using System.Security.Claims;

namespace Pastebug.WebApi.utils;

public class CurrentUserChecker : ICurrentUserChecker
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUserService _userService;

    public CurrentUserChecker(IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _contextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public async Task<Guid> UserId()
    {
        var email = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        Guid userGuid = await  _userService.GetUserIdAsync(email);

        return userGuid;
    }
}
