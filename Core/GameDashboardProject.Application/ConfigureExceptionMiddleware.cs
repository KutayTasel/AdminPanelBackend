
using GameDashboardProject.Application.Exceptions;
using Microsoft.AspNetCore.Builder;

namespace GameDashboardProject.Application
{
    public static class ConfigureExceptionMiddleware
    {
        public static void ConfigureExceptionHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
