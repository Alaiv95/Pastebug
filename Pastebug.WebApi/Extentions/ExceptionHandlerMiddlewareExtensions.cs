using Pastebug.WebApi.Middlewares;

namespace Pastebug.WebApi.Extentions;

public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseMyExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddlware>();
    }
}
