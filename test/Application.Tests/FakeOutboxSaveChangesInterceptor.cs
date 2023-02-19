using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using ChatApp.Infrastructure.Persistence.Outbox;
using System.Threading.Tasks;
using System.Threading;
using ChatApp.Domain.Entities;
using ChatApp.Services;
using System.Linq;

namespace ChatApp.Infrastructure.Persistence.Interceptors;

public sealed class FakeOutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher domainEventDispatcher;

    public FakeOutboxSaveChangesInterceptor(IDomainEventDispatcher domainEventDispatcher)
    {
        this.domainEventDispatcher = domainEventDispatcher;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var entities = context.ChangeTracker
                        .Entries<IHasDomainEvents>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        await Task.WhenAll(domainEvents.Select(x => domainEventDispatcher.Dispatch(x)));

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}