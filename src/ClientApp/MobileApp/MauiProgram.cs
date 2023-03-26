using Microsoft.Extensions.Logging;
using ChatApp.Data;
using ChatApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ChatApp.Auth0;

namespace ChatApp;

public static class MauiProgram
{
    public static Microsoft.Maui.Hosting.MauiApp CreateMauiApp()
    {
        var builder = Microsoft.Maui.Hosting.MauiApp.CreateBuilder();
        builder
            .UseMauiApp<Microsoft.Maui.Hosting.MauiApp>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddChatApp<CustomAuthorizationMessageHandler>();

        //builder.Services.AddScoped<ChatApp.Services.IAccessTokenProvider, ChatApp.Services.AccessTokenProvider>();

        //builder.Services.AddScoped<ChatApp.Services.ICurrentUserService, ChatApp.Services.CurrentUserService>();

        // 👇 new code
        builder.Services.AddSingleton(new Auth0Client(new()
        {
            Domain = builder.Configuration["Local:Authority"],
            ClientId = builder.Configuration["Local:ClientId"],
            Scope = builder.Configuration["Local:DefaultScopes"],
            RedirectUri = "myapp://callback"
        }));
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, Auth0AuthenticationStateProvider>();
        // 👆 new code

        return builder.Build();
    }
}

public sealed class CustomAuthorizationMessageHandler : DelegatingHandler
{
    
}