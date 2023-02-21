using ChatApp.Features.Users;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Features.Chat;

public interface IDtoFactory 
{
    IEnumerable<MessageDto> GetMessageDtos(Message[] messages);
}

public sealed class DtoFactory : IDtoFactory
{
    private readonly ApplicationDbContext context;

    public DtoFactory(ApplicationDbContext context)
    {
        this.context = context;
    }

    public IEnumerable<MessageDto> GetMessageDtos(Message[] messages)
    {
        var groups = messages.Join(
                        context.Users,
                        m => (string?)m.CreatedById,
                        u => (string)u.Id,
                        (Message, CreatedBy) => new
                        {
                            Message,
                            CreatedBy
                        });

        var messagesWithReplies = groups
            .Select(x => x.Message)
            .Where(x => x.ReplyToId != null);

        Dictionary<Message, Message> repliedMessages = new Dictionary<Message, Message>();

        if(messagesWithReplies.Any()) 
        {
            repliedMessages = messagesWithReplies
                .Join(context.Messages, x => x.ReplyToId, x => x.Id, 
                (message, reply) => new {
                    message,
                    reply
                }).ToDictionary(x => x.message, x => x.reply);
        }

        return groups.Select(g => {
            repliedMessages.TryGetValue(g.Message, out var replyMessage);

            return new MessageDto(
                g.Message.Id, g.Message.ChannelId, 
                replyMessage is null ? null 
                : new ReplyMessageDto(
                    (Guid)replyMessage.ReplyToId.GetValueOrDefault(), 
                    replyMessage?.ChannelId ?? Guid.NewGuid(), 
                    replyMessage?.Content ?? string.Empty,
                    replyMessage!.Published, new UserDto(replyMessage.CreatedById.ToString()!, string.Empty),
                    replyMessage!.LastModified, null, 
                    replyMessage!.Deleted, null), 
                g.Message.Content, g.Message.Created, new UserDto(g.CreatedBy.Id.ToString(), g.CreatedBy.Name), 
                g.Message.LastModified, null, 
                g.Message.Deleted, null); 
        }).ToList();
    }
}
