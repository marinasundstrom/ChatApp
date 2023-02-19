using ChatApp.Features.Chat.Messages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace ChatApp.Features.Chat;

public interface IChatNotificationService
{
    Task NotifyMessagePosted(MessageDto message, CancellationToken cancellationToken = default);
    Task SendMessageToUser(string userId, MessageDto message, CancellationToken cancellationToken = default);
    Task SendConfirmationToSender(string channelId, string senderId, string messageId, CancellationToken cancellationToken = default);
    Task NotifyMessageEdited(string channelId, string messageId, string content, CancellationToken cancellationToken = default);
    Task NotifyMessageDeleted(string channelId, string messageId, CancellationToken cancellationToken = default);
}

public class ChatNotificationService : IChatNotificationService
{
    private readonly IHubContext<ChatHub, IChatHubClient> hubsContext;

    public ChatNotificationService(IHubContext<ChatHub, IChatHubClient> hubsContext)
    {
        this.hubsContext = hubsContext;
    }

    public async Task NotifyMessagePosted(MessageDto message, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .Group($"channel-{message.ChannelId}")
            .MessagePosted(message);
    }

    public async Task SendMessageToUser(string userId, MessageDto message, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .User(userId)
            .MessagePosted(message);
    }

    public async Task SendConfirmationToSender(string channelId, string senderId, string messageId, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .User(senderId)
            .MessagePostedConfirmed(messageId);
    }

    public async Task NotifyMessageEdited(string channelId, string messageId, string content, CancellationToken cancellationToken = default) 
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            .MessageEdited(channelId, messageId, content);
    }

    public async Task NotifyMessageDeleted(string channelId, string messageId, CancellationToken cancellationToken = default) 
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            .MessageDeleted(channelId, messageId);
    }
}