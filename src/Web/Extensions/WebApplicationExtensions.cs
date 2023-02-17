using ChatApp.Features.Channels;
using ChatApp.Features.Users;

namespace ChatApp.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.MapMessageEndpoints();
        app.MapUsersEndpoints();

        return app;
    }

    public static WebApplication MapApplicationHubs(this WebApplication app)
    {
        app.MapTodoHubs();

        return app;
    }
}
