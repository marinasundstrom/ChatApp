using Microsoft.Extensions.Caching.Distributed;

namespace ChatApp.Features.Chat;

public record CachedMessageSender(string UserId, string ConnectionId);

public interface IMessageSenderCache
{
    Task<CachedMessageSender> GetSenderConnectionId(string messageId, CancellationToken cancellationToken = default);

    Task StoreSenderConnectionId(string messageId, string userId, string connectionId, CancellationToken cancellationToken = default);

    Task RemoveCachedSenderConnectionId(string messageId, CancellationToken cancellationToken = default);
}

public sealed class MessageSenderCache : IMessageSenderCache
{
    private readonly IDistributedCache distributedCache;

    public MessageSenderCache(IDistributedCache distributedCache)
    {
        this.distributedCache = distributedCache;
    }

    public async Task<CachedMessageSender> GetSenderConnectionId(string messageId, CancellationToken cancellationToken = default)
    {
        return await distributedCache.GetAsync<CachedMessageSender>(messageId, cancellationToken);
    }

    public async Task StoreSenderConnectionId(string messageId, string userId, string connectionId, CancellationToken cancellationToken = default)
    {
        await distributedCache.SetAsync(messageId, new CachedMessageSender(userId, connectionId!), new DistributedCacheEntryOptions(), cancellationToken);
    }

    public async Task RemoveCachedSenderConnectionId(string messageId, CancellationToken cancellationToken = default)
    {
        await distributedCache.RemoveAsync(messageId);
    }
}