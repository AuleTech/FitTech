using System.Text.Json;
using FastEndpoints;
using Microsoft.AspNetCore.Diagnostics;

namespace FitTech.API;

class GlobalExceptionHandler;

public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app,
        ILogger? logger = null,
        bool logStructuredException = false,
        bool useGenericReason = false)
    {
        app.UseExceptionHandler(errApp =>
        {
            errApp.Run(async ctx =>
            {
                var exHandlerFeature = ctx.Features.Get<IExceptionHandlerFeature>();

                if (exHandlerFeature is not null)
                {
                    logger ??= ctx.Resolve<ILogger<GlobalExceptionHandler>>();
                    var route = exHandlerFeature.Endpoint?.DisplayName?.Split(" => ")[0];
                    var exceptionType = exHandlerFeature.Error.GetType().Name;
                    var reason = exHandlerFeature.Error.Message;

                    ctx.Response.StatusCode = exHandlerFeature.Error is UnauthorizedAccessException ? 401 : 500;
                    await ctx.Response.WriteAsJsonAsync(
                        new InternalErrorResponse
                        {
                            Status = "Internal Server Error!",
                            Code = ctx.Response.StatusCode,
                            Reason = useGenericReason ? "An unexpected error has occurred." : reason,
                            Note = "See application log for stack trace."
                        },
                        new JsonSerializerOptions(),
                        "application/problem+json",
                        ctx.RequestAborted);
                }
            });
        });

        return app;
    }
}
