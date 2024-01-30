using Pastebug.Domain.Entities;

namespace Pastebug.BLL.auth;

public interface IJwtToken
{
    public string Generate(User user);
}
