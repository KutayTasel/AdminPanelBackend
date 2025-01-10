using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameDashboardProject.Application.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var status = GetStatusCode(exception);
            context.Response.StatusCode = status;

            if (exception is ValidationException validationException)
            {
                var response = new ValidationErrorResponse
                {
                    Title = "Validation Failed",
                    Status = status,
                    Errors = validationException.Errors
                        .Select(e => new ValidationError
                        {
                            PropertyName = e.PropertyName,
                            ErrorMessage = e.ErrorMessage
                        })
                        .ToList()
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Status = status,
                    Detail = exception.Message
                };

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }

        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                NotFoundException => StatusCodes.Status404NotFound,
                BadRequestException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
    }
}

