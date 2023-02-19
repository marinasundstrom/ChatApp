namespace ChatApp.Features.Chat;

public static class ServiceExtensions
{
    public static IServiceCollection AddTodoControllers(this IServiceCollection services)
    {
        services.AddScoped<IAdminCommandProcessor, AdminCommandProcessor>();

        return services;
    }
}
