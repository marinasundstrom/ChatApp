using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.IntegrationTests;

partial class MessagesTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    /*
    [Fact]
    public async Task ShouldGetNotificationWhenMessageIsCreated()
    {
        // Arrange

        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("JWT");

        var hubConnection = new HubConnectionBuilder()
            .WithUrl($"http://localhost/hubs/messages", o => o.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler())
            .WithAutomaticReconnect().Build();

        var completion = new ManualResetEvent(false);

        int? receivedId = null;

        hubConnection.On<int, string>("Created", (id, title) =>
        {
            receivedId = id;
            completion.Set();
        });

        await hubConnection.StartAsync();

        MessagesClient messagesClient = new(client);

        string title = "Foo Bar";
        string description = "Lorem ipsum";
        MessageStatus status = MessageStatus.InProgress;

        // Act

        var message = await messagesClient.CreateMessageAsync(new CreateMessageRequest()
        {
            Title = title,
            Description = description,
            Status = status
        });

        completion.WaitOne();

        await hubConnection.StopAsync();

        // Assert

        receivedId.Should().NotBeNull();
        receivedId.Should().Be(message.Id);
    }
    */
}