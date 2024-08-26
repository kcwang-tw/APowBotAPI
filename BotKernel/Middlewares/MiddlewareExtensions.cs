namespace BotKernel.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApiAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiAuthMiddleware>();
        }
    }
}
