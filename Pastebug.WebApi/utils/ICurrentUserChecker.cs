
namespace Pastebug.WebApi.utils
{
    public interface ICurrentUserChecker
    {
        Task<Guid> UserId();
    }
}