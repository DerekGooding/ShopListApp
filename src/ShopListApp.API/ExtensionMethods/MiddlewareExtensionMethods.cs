namespace ShopListApp.API.ExtensionMethods;

public static class MiddlewareExtensionMethods
{
    public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app) => app.UseMiddleware<ExceptionHandlerMiddleware>();
}
