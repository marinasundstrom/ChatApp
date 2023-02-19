namespace ChatApp.Features.Chat;

public interface IMessageSenderCacheService
{
    Task<CachedMessageSender> GetSenderConnectionId(string messageId, CancellationToken cancellationToken = default);

    Task RemoveCachedSenderConnectionId(string messageId, CancellationToken cancellationToken = default);

    Task StoreSenderConnectionId(string messageId, CancellationToken cancellationToken = default);
}

public sealed class MessageSenderCacheService : IMessageSenderCacheService
{
    private readonly IMessageSenderCache messageSenderCache;
    private readonly ICurrentUserService currentUserService;

    public MessageSenderCacheService(IMessageSenderCache messageSenderCache, ICurrentUserService currentUserService)
    {
        this.messageSenderCache = messageSenderCache;
        this.currentUserService = currentUserService;
    }

    public async Task<CachedMessageSender> GetSenderConnectionId(string messageId, CancellationToken cancellationToken = default)
    {
        return await messageSenderCache.GetSenderConnectionId(messageId, cancellationToken);
    }

    public async Task StoreSenderConnectionId(string messageId, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId;
        var connectionId = currentUserService.ConnectionId;

        await messageSenderCache.StoreSenderConnectionId(messageId, userId!, connectionId!, cancellationToken);
    }

    public async Task RemoveCachedSenderConnectionId(string messageId, CancellationToken cancellationToken = default)
    {
        await messageSenderCache.RemoveCachedSenderConnectionId(messageId, cancellationToken);
    }
}