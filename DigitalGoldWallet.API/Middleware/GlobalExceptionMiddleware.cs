using System.Net;
using System.Text.Json;
using DigitalGoldWallet.API.Exceptions;

namespace DigitalGoldWallet.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(
        RequestDelegate next)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        HttpStatusCode statusCode =
            HttpStatusCode.InternalServerError;

        var message = exception.Message;

        switch (exception)
        {
            case BadRequestException:
                statusCode = HttpStatusCode.BadRequest;
                break;

            case UnauthorizedException:
                statusCode = HttpStatusCode.Unauthorized;
                break;

            case ForbiddenException:
                statusCode = HttpStatusCode.Forbidden;
                break;

            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                break;

            case ConflictException:
                statusCode = HttpStatusCode.Conflict;
                break;
        }

        var response = new
        {
            Message = message
        };

        context.Response.ContentType =
            "application/json";

        context.Response.StatusCode =
            (int)statusCode;

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}