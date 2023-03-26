using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FeatureManagement;
using MudBlazor.Services;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using ChatApp;
using ChatApp.Theming;
using ChatApp.Chat;
using ChatApp.Chat.Messages;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddChatApp<CustomAuthorizationMessageHandler>();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Local", options.ProviderOptions);
});

builder.Services.AddScoped<ChatApp.Services.IAccessTokenProvider, ChatApp.Services.AccessTokenProvider>();

builder.Services.AddScoped<ChatApp.Services.ICurrentUserService, ChatApp.Services.CurrentUserService>();

var app = builder.Build();

await app.Services.Localize();

await app.RunAsync();