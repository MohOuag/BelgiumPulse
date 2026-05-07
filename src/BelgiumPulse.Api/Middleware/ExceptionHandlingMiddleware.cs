using FluentValidation;
using BelgiumPulse.Application.Common.Exceptions;
using System.Text.Json;

namespace BelgiumPulse.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            await WriteErrorResponse(context, 404, ex.Message);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("Validation échouée: {Errors}", errors);
            await WriteErrorResponse(context, 400, "Validation échouée", errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue");
            await WriteErrorResponse(context, 500, "Une erreur interne est survenue");
        }
    }

    private static async Task WriteErrorResponse(
        HttpContext context,
        int statusCode,
        string message,
        List<string>? errors = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = statusCode,
            message,
            errors
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}