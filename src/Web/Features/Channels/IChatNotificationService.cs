using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace ChatApp.Features.Channels;

public interface IChatNotificationService
{
    Task MessagePosted(MessageDto message, CancellationToken cancellationToken = default);
    Task MessageEdited(string channelId, string messageId, string content, CancellationToken cancellationToken = default);
    Task MessageDeleted(string channelId, string messageId, CancellationToken cancellationToken = default);
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

    public async Task MessagePosted(MessageDto message, CancellationToken cancellationToken = default)
    {
        string senderConnectionId = await GetSenderConnectionId(message, cancellationToken);

        await hubsContext.Clients
            .GroupExcept($"channel-{message.ChannelId}", senderConnectionId)
            .MessagePosted(message);

        await distributedCache.RemoveAsync(message.Id.ToString());
    }

    private async Task<string> GetSenderConnectionId(MessageDto message, CancellationToken cancellationToken)
    {
        return await distributedCache.GetAsync<string>(message.Id.ToString(), cancellationToken);
    }

    public async Task MessageEdited(string channelId, string messageId, string content, CancellationToken cancellationToken = default) 
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            //.GroupExcept($"channel-{channelId}", Context.ConnectionId)
            .MessageEdited(channelId, messageId, content);
    }

    public async Task MessageDeleted(string channelId, string messageId, CancellationToken cancellationToken = default) 
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            //.GroupExcept($"channel-{channelId}", Context.ConnectionId)
            .MessageDeleted(channelId, messageId);
    }
}