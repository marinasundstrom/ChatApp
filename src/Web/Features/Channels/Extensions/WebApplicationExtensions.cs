namespace ChatApp.Features.Channels;

public static class WebApplicationExtensions
{
    public static WebApplication MapTodoHubs(this WebApplication app)
    {
        app.MapHub<ChatHub>("/hubs/chat");

        return app;
    }
}
