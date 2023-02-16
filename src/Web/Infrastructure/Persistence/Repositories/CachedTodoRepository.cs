using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ChatApp.Domain.Specifications;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Infrastructure.Persistence.Repositories;

public sealed class CachedTodoRepository : IMessageRepository
{
    private readonly IMessageRepository decorated;
    private readonly IMemoryCache memoryCache;

    public CachedTodoRepository(IMessageRepository decorated, IMemoryCache memoryCache)
    {
        this.decorated = decorated;
        this.memoryCache = memoryCache;
    }

    public void Add(Message item)
    {
        decorated.Add(item);
    }

    public async Task<Message?> FindByIdAsync(MessageId id, CancellationToken cancellationToken = default)
    {
        string key = $"todo-{id}";

        return await memoryCache.GetOrCreateAsync<Message?>(key, async options =>
        {
            options.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(2);

            return await decorated.FindByIdAsync(id, cancellationToken);
        });
    }

    public IQueryable<Message> GetAll()
    {
        return decorated.GetAll();
    }

    public IQueryable<Message> GetAll(ISpecification<Message> specification)
    {
        return decorated.GetAll(specification);
    }

    public void Remove(Message item)
    {
        decorated.Remove(item);
    }
}
