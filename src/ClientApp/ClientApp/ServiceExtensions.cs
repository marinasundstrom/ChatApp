using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FeatureManagement;
using MudBlazor.Services;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using ChatApp;
using ChatApp.Theming;
using ChatApp.Chat;
using ChatApp.Chat.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp;

public static class ServiceExtensions
{
    public static IServiceCollection AddChatApp<TMessageHandler>(this IServiceCollection services)
        where TMessageHandler : DelegatingHandler
    {
        services.AddFeatureManagement();

        services.AddTransient<TMessageHandler>();

        services.AddHttpClient("WebAPI",
                client => client.BaseAddress = new Uri("https://localhost:5001/"));

        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
            .CreateClient("WebAPI"));

        services.AddHttpClient<IChannelsClient>(nameof(ChannelsClient), (sp, http) =>
        {
            http.BaseAddress = new Uri("https://localhost:5001/");
        })
        .AddTypedClient<IChannelsClient>((http, sp) => new ChannelsClient(http))
        .AddHttpMessageHandler<TMessageHandler>()
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
        .AddPolicyHandler(GetRetryPolicy());

        services.AddHttpClient<IMessagesClient>(nameof(MessagesClient), (sp, http) =>
        {
            http.BaseAddress = new Uri("https://localhost:5001/");
        })
        .AddTypedClient<IMessagesClient>((http, sp) => new MessagesClient(http))
        .AddHttpMessageHandler<TMessageHandler>()
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
        .AddPolicyHandler(GetRetryPolicy());

        services.AddHttpClient<IUsersClient>(nameof(UsersClient), (sp, http) =>
        {
            http.BaseAddress = new Uri("https://localhost:5001/");
        })
        .AddTypedClient<IUsersClient>((http, sp) => new UsersClient(http))
        .AddHttpMessageHandler<TMessageHandler>();
        //.SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
        //.AddPolicyHandler(GetRetryPolicy());

        services.AddMudServices();

        services.AddBlazoredLocalStorage();

        services.AddThemeServices();

        services.AddLocalization();

        services.AddSingleton<ITimeViewService, TimeViewService>();

        return services;
    }

    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
             .HandleTransientHttpError()
             .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5));
    }

}
