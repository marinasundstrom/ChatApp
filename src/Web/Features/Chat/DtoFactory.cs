using ChatApp.Domain.ValueObjects;
using ChatApp.Features.Users;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Features.Chat;

public interface IDtoFactory
{
    Task<IEnumerable<MessageDto>> GetMessageDtos(Message[] messages, CancellationToken cancellationToken = default);
}

public sealed class DtoFactory : IDtoFactory
{
    private readonly ApplicationDbContext context;

    public DtoFactory(ApplicationDbContext context)
    {
        this.context = context;
    }


    public async Task<IEnumerable<MessageDto>> GetMessageDtos(Message[] messages, CancellationToken cancellationToken = default)
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

                replyMessageDto = new ReplyMessageDto(
                    (Guid)replyMessage.Id,
                    replyMessage.ChannelId,
                    replyMessage.Content,
                    replyMessage.Created, new UserDto(replyMessagePublishedBy!.Id.ToString(), replyMessagePublishedBy.Name),
                    replyMessage.LastModified, replyMessage.LastModifiedById is null ? null : new UserDto(replyMessageEditedBy!.Id.ToString(), replyMessageEditedBy.Name),
                    replyMessage.Deleted, replyMessage.DeletedById is null ? null : new UserDto(replyMessageDeletedBy!.Id.ToString(), replyMessageDeletedBy.Name));
            }

            return new MessageDto(
                message.Id,
                message.ChannelId,
                replyMessageDto,
                message.Content,
                message.Created, new UserDto(publishedBy!.Id.ToString(), publishedBy.Name),
                message.LastModified, message.LastModifiedById is null ? null : new UserDto(editedBy!.Id.ToString(), editedBy.Name),
                message.Deleted, message.DeletedById is null ? null : new UserDto(deletedBy!.Id.ToString(), deletedBy.Name));
        });
    }
}
