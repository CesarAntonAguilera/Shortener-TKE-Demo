using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Shortener_TKE_Demo.Middlewares
{
    public sealed class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request: {Message}", ex.Message);
                await WriteProblemDetails(context, HttpStatusCode.BadRequest, "Validation error", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteProblemDetails(context, HttpStatusCode.InternalServerError, "Server error",
                    "An unexpected error occurred.");
            }
        }

        private static async Task WriteProblemDetails(
            HttpContext context,
            HttpStatusCode statusCode,
            string title,
            string detail)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
