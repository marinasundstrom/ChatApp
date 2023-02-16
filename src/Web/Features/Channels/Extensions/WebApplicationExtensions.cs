namespace ChatApp.Features.Channels;

public static class WebApplicationExtensions
{
    public static WebApplication MapTodoHubs(this WebApplication app)
    {
        app.MapHub<TodosHub>("/hubs/todos");

        return app;
    }
}
