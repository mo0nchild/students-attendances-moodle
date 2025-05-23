using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Attendances.Application.Commons.Exceptions;

namespace Attendances.Shared.Commons.Middlewares;

public record class ExceptionMessage(string Title, string Errors);
internal class ExceptionsMiddleware(RequestDelegate next, ILogger<ExceptionsMiddleware> logger)
{
    protected ILogger<ExceptionsMiddleware> Logger { get;  } = logger;

    public async Task Invoke(HttpContext context)
    {
        try { await next.Invoke(context); }
        catch (ProcessException error)
        {
            Logger.LogWarning($"An process exception occurred during the request: {error.GetType().Name}");
            Logger.LogWarning($"Exception message: {error.Message}");

            await HandleExceptionAsync(context, error);
        }
        catch (ValidationException error)
        {
            Logger.LogWarning($"An validation exception occurred during the request: {error.GetType().Name}");
            Logger.LogWarning($"Exception message: {error.Message}");

            await HandleExceptionAsync(context, error);
        }
        catch (Exception error)
        {
            Logger.LogError($"An exception occurred during the request {error.GetType().Name}: {error.Message}");
            Logger.LogError(error.StackTrace);
        }
    }
    protected virtual async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var result = new ExceptionMessage(
            Title: $"Exception type: {exception.GetType().Name}",
            Errors: exception.Message
        );  
        await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
    }
}
public static class ExceptionsMiddlewareExtension
{
    internal static IApplicationBuilder UseExceptionsHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionsMiddleware>();
    }
}