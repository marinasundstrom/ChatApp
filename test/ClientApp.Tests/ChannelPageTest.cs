using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using NSubstitute;
using ChatApp;
using ChatApp.Pages;

namespace ChatApp.Tests;

public class ChannelPageTest
{
    [Fact]
    public void MessagesShouldLoadOnInitializedSuccessful()
    {
        // Arrange
        using var ctx = new TestContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;

        ctx.Services.AddMudServices();
        ctx.Services.AddLocalization();

        var fakeThemeManager = Substitute.For<ChatApp.Theming.IThemeManager>();
        ctx.Services.AddSingleton(fakeThemeManager);

        var fakeAccessTokenProvider = Substitute.For<ChatApp.Services.IAccessTokenProvider>();
        ctx.Services.AddSingleton(fakeAccessTokenProvider);

        var fakeCurrentUserService = Substitute.For<ChatApp.Services.ICurrentUserService>();
        fakeCurrentUserService.GetUserIdAsync().Returns("foo");
        ctx.Services.AddSingleton(fakeCurrentUserService);

        var fakeTimeViewService = Substitute.For<ChatApp.Chat.Messages.ITimeViewService>();
        ctx.Services.AddSingleton(fakeTimeViewService);

        var fakeMessagesClient = Substitute.For<IMessagesClient>();
        fakeMessagesClient.GetMessagesAsync(Arg.Any<Guid>(), null, null, null, default)
            .ReturnsForAnyArgs(t => new ItemsResultOfMessage()
            {
                Items = new[]
                {
                    new Message
                    {
                        Id = Guid.NewGuid(),
                        Content = "Hello world",
                        Created = DateTimeOffset.Now.AddMinutes(-3),
                        CreatedBy = new User {
                            Id = "1",
                            Name = "Foo"
                        }
                    },
                },
                TotalItems = 3
            });

        ctx.Services.AddSingleton<IMessagesClient>(fakeMessagesClient);

        var cut = ctx.RenderComponent<ChatApp.Chat.Channels.ChannelPage>();

        // Act
        //cut.Find("button").Click();

        // Assert
        cut.WaitForState(() => cut.Find("div.message") != null);
    }
}
