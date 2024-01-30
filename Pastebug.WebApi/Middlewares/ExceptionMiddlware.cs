using Pastebug.BLL.Exceptions;
using System.Net;
using System.Text.Json;

namespace Pastebug.WebApi.Middlewares;

public class ExceptionMiddlware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddlware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleException(ex, context);
        }
    }

    private Task HandleException(Exception ex, HttpContext context)
    {
        int code = (int)HttpStatusCode.InternalServerError;

        if (ex is ApiException exception)
        {
            code = exception.Code;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;

        var result = JsonSerializer.Serialize(new { error = ex.Message });

        return context.Response.WriteAsync(result);
    }
}
