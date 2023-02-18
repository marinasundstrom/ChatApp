using Microsoft.Extensions.Caching.Distributed;

namespace ChatApp.Features.Channels;

public record CachedMessageSender(string UserId, string ConnectionId);

public interface IMessageSenderCache
{
    Task<CachedMessageSender> GetSenderConnectionId(string messageId, CancellationToken cancellationToken);

    Task StoreSenderConnectionId(string messageId, string userId, string connectionId, CancellationToken cancellationToken);
}

public sealed class MessageSenderCache : IMessageSenderCache
{
    private readonly IDistributedCache distributedCache;

    public MessageSenderCache(IDistributedCache distributedCache)
    {
        this.distributedCache = distributedCache;
    }

    public async Task<CachedMessageSender> GetSenderConnectionId(string messageId, CancellationToken cancellationToken)
    {
        return await distributedCache.GetAsync<CachedMessageSender>(messageId, cancellationToken);
    }

    public async Task StoreSenderConnectionId(string messageId, string userId, string connectionId, CancellationToken cancellationToken)
    {
        await distributedCache.SetAsync(messageId, new CachedMessageSender(userId, connectionId!), new DistributedCacheEntryOptions(), cancellationToken);
    }
}