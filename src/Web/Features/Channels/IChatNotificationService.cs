using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace ChatApp.Features.Channels;

public interface IChatNotificationService
{
    Task NotifyMessagePosted(MessageDto message, CancellationToken cancellationToken = default);
    Task SendConfirmationToSender(string channelId, string messageId, CancellationToken cancellationToken = default);
    Task NotifyMessageEdited(string channelId, string messageId, string content, CancellationToken cancellationToken = default);
    Task NotifyMessageDeleted(string channelId, string messageId, CancellationToken cancellationToken = default);
}

public class ChatNotificationService : IChatNotificationService
{
    private readonly IHubContext<ChatHub, IChatHubClient> hubsContext;
    private readonly IDistributedCache distributedCache;

    public ChatNotificationService(
        IHubContext<ChatHub, IChatHubClient> hubsContext, 
        IDistributedCache distributedCache)
    {
        this.hubsContext = hubsContext;
        this.distributedCache = distributedCache;
    }

    public async Task NotifyMessagePosted(MessageDto message, CancellationToken cancellationToken = default)
    {
        var (UserId, ConnectionId) = await GetSenderConnectionId(message.Id.ToString(), cancellationToken);

        await hubsContext.Clients
            .GroupExcept($"channel-{message.ChannelId}", ConnectionId)
            .MessagePosted(message);
    }

    public async Task SendConfirmationToSender(string channelId, string messageId, CancellationToken cancellationToken = default)
    {
        var (UserId, ConnectionId) = await GetSenderConnectionId(messageId, cancellationToken);

        await hubsContext.Clients
            .User(UserId)
            .MessagePostedConfirmed(messageId);
    }

    private async Task<CachedMessageSender> GetSenderConnectionId(string messageId, CancellationToken cancellationToken)
    {
        return await distributedCache.GetAsync<CachedMessageSender>(messageId, cancellationToken);
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