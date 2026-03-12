using Domain.Models.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LMS.API.Extensions;

public static class ExceptionMiddlewareExtetensions
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var problemDetailsFactory = app.Services.GetRequiredService<ProblemDetailsFactory>();
                    var exception = contextFeature.Error;

                    var (statusCode, title) = exception switch
                    {
                        BadRequestException => (StatusCodes.Status400BadRequest, (exception as BadRequestException)!.Title),
                        NotFoundException => (StatusCodes.Status404NotFound, (exception as NotFoundException)!.Title),
                        TokenValidationException tokenEx => (tokenEx.StatusCode, "Unauthorized"),
                        _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
                    };

                    var problemDetails = problemDetailsFactory.CreateProblemDetails(
                        context,
                        statusCode,
                        title: title,
                        detail: exception.Message,
                        instance: context.Request.Path);

                    context.Response.StatusCode = statusCode;
                    await context.Response.WriteAsJsonAsync(problemDetails);
                }
            });
        });
    }
}
