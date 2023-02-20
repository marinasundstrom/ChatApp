using System;
using System.Net.Http.Headers;
using MassTransit;
using MassTransit.Testing;
using ChatApp.Contracts;
using static MassTransit.Logging.OperationName;
using System.Threading.Tasks;

namespace ChatApp.IntegrationTests;

partial class MessagesTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    /*
    [Fact]
    public async Task UpdateStatusConsumed()
    {
        // Arrange

        var harness = _factory.Services.GetTestHarness();

        await harness.Start();

        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("JWT");

        MessagesClient messagesClient = new(client);

        string title = "Foo Bar";
        string description = "Lorem ipsum";
        Contracts.MessageStatus status = Contracts.MessageStatus.InProgress;

        var newStatus = Contracts.MessageStatus.Completed;

        var message = await messagesClient.CreateMessageAsync(new CreateMessageRequest()
        {
            Title = title,
            Description = description,
            Status = (MessageStatus)status
        });

        // Act

        await harness.Bus.Publish(
            new UpdateStatus(message.Id, (Contracts.MessageStatus)newStatus));

        // Assert

        Assert.True(await harness.Consumed.Any<UpdateStatus>());

        var message2 = await messagesClient.GetMessageByIdAsync(message.Id);

        message2.Status.Should().Be((MessageStatus)newStatus);
    }
    */
}

