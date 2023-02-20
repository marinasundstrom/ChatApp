using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ChatApp.IntegrationTests;

public partial class MessagesTest : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public MessagesTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostMessages_ShouldBeRetrievedByItsId()
    {
        // Arrange

        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("JWT");

        MessagesClient messagesClient = new(client);

        string content = "Foo Bar";

        // Act

        var messageId = await messagesClient.PostMessageAsync(new PostMessageRequest()
        {
            ChannelId = Utilities.ChannelId,
            Content = content
        });

        var message = await messagesClient.GetMessageByIdAsync(messageId);

        // Assert
        message.Id.Should().Be(messageId);
        message.Content.Should().Be(content);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("JWT");

        try
        {
            UsersClient usersClient = new(client);

            var user = await usersClient.CreateUserAsync(new CreateUser()
            {
                Name = "Test",
                Email = "test@email.com"
            });
        }
        catch { }
    }

    [Fact]
    public async Task NonExistentIdShouldReturnNotFound()
    {
        // Arrange

        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("JWT");

        MessagesClient messagesClient = new(client);

        Guid nonExistentId = Guid.NewGuid();

        // Act

        var exception = await Assert.ThrowsAsync<ApiException>(async () =>
        {
            var message = await messagesClient.GetMessageByIdAsync(nonExistentId);
        });

        // Assert

        exception.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }
}