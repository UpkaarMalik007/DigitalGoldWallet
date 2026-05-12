using DigitalGoldWallet.API.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace DigitalGoldWallet.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        object response;

        switch (exception)
        {
            case BadRequestException:
                statusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    statusCode,
                    message = exception.Message
                };
                break;

            case UnauthorizedException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                response = new
                {
                    statusCode,
                    message = exception.Message
                };
                break;

            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                response = new
                {
                    statusCode,
                    message = exception.Message
                };
                break;

            case ForbiddenException:
                statusCode = (int)HttpStatusCode.Forbidden;
                response = new
                {
                    statusCode,
                    message = exception.Message
                };
                break;

            case NotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                response = new
                {
                    statusCode,
                    message = exception.Message
                };
                break;

            case ConflictException:
                statusCode = (int)HttpStatusCode.Conflict;
                response = new
                {
                    statusCode,
                    message = exception.Message
                };
                break;

            case InvalidOperationException:
                statusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    statusCode,
                    message = exception.Message
                };
                break;

            case ValidationException validationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    statusCode,
                    message = "Validation failed.",
                    errors = validationException.Errors
                        .Select(error => new
                        {
                            field = error.PropertyName,
                            message = error.ErrorMessage
                        })
                        .ToList()
                };
                break;
            case InvalidOperationException:
                statusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    statusCode,
                    message = exception.Message
                };
                break;



            case InvalidOperationException:
                statusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    statusCode,
                    message = exception.Message
                };
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;

                _logger.LogError(exception, "An unhandled exception occurred.");

                response = new
                {
                    statusCode,
                    message = "An unexpected error occurred. Please try again later."
                };
                break;
        }

        context.Response.StatusCode = statusCode;

        string jsonResponse = JsonSerializer.Serialize(
            response,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        await context.Response.WriteAsync(jsonResponse);
    }
}