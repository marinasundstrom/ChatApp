using ChatApp.Domain.ValueObjects;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Features.Chat;

public interface IDtoComposer
{
    Task<MessageDto> ComposeMessageDto(Message message, CancellationToken cancellationToken = default);
    Task<IEnumerable<MessageDto>> ComposeMessageDtos(Message[] messages, CancellationToken cancellationToken = default);
}

public sealed class DtoComposer : IDtoComposer
{
    private readonly ApplicationDbContext context;
    private readonly IDtoFactory dtoFactory;

    public DtoComposer(ApplicationDbContext context, IDtoFactory dtoFactory)
    {
        this.context = context;
        this.dtoFactory = dtoFactory;
    }

    public async Task<MessageDto> ComposeMessageDto(Message message, CancellationToken cancellationToken = default)
    {
        HashSet<UserId> userIds = new();
        HashSet<MessageId> messageIds = new();

        if (message.ReplyToId is not null)
        {
            messageIds.Add(message.ReplyToId.GetValueOrDefault());
        }

        if (message.CreatedById is not null)
        {
            userIds.Add(message.CreatedById.GetValueOrDefault());
        }

        if (message.LastModifiedById is not null)
        {
            userIds.Add(message.LastModifiedById.GetValueOrDefault());
        }

        if (message.DeletedById is not null)
        {
            userIds.Add(message.DeletedById.GetValueOrDefault());
        }

        var users = await context.Users
            .Where(x => userIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var repliedMessages = await context.Messages
            .Where(x => messageIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        return ComposeMessageDtoInternal(message, users, repliedMessages);
    }

    public async Task<IEnumerable<MessageDto>> ComposeMessageDtos(Message[] messages, CancellationToken cancellationToken = default)
    {
        HashSet<UserId> userIds = new();
        HashSet<MessageId> messageIds = new();

        foreach (var message in messages)
        {
            if (message.ReplyToId is not null)
            {
                messageIds.Add(message.ReplyToId.GetValueOrDefault());
            }

            if (message.CreatedById is not null)
            {
                userIds.Add(message.CreatedById.GetValueOrDefault());
            }

            if (message.LastModifiedById is not null)
            {
                userIds.Add(message.LastModifiedById.GetValueOrDefault());
            }

            if (message.DeletedById is not null)
            {
                userIds.Add(message.DeletedById.GetValueOrDefault());
            }
        }

        var users = await context.Users
            .Where(x => userIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var repliedMessages = await context.Messages
            .Where(x => messageIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        return messages.Select(message =>
        {
            return ComposeMessageDtoInternal(message, users, repliedMessages);
        });
    }

    private MessageDto ComposeMessageDtoInternal(Message message, Dictionary<UserId, User> users, Dictionary<MessageId, Message> repliedMessages)
    {
        repliedMessages.TryGetValue(message.ReplyToId.GetValueOrDefault(), out var replyMessage);

        users.TryGetValue(message.CreatedById.GetValueOrDefault(), out var publishedBy);

        users.TryGetValue(message.LastModifiedById.GetValueOrDefault(), out var editedBy);

        users.TryGetValue(message.DeletedById.GetValueOrDefault(), out var deletedBy);

        ReplyMessageDto? replyMessageDto = null;

        if (replyMessage is not null)
        {
            users.TryGetValue(replyMessage.CreatedById.GetValueOrDefault(), out var replyMessagePublishedBy);

            users.TryGetValue(replyMessage.LastModifiedById.GetValueOrDefault(), out var replyMessageEditedBy);

            users.TryGetValue(replyMessage.DeletedById.GetValueOrDefault(), out var replyMessageDeletedBy);

            replyMessageDto = dtoFactory.CreateReplyMessageDto(replyMessage, replyMessagePublishedBy!, replyMessageEditedBy, replyMessageDeletedBy);
        }

        return dtoFactory.CreateMessageDto(message, publishedBy!, editedBy, deletedBy, replyMessageDto);
    }
}
