
namespace Catalog.API.Extensions;

public static class ProgramExtensionApp
{
    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
