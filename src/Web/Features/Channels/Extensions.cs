using ChatApp.Features.Users;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Features.Channels;

public static class Extensions
{
    public static IEnumerable<MessageDto> GetMessageDtos(DbSet<User> users, Message[] messages)
    {
        var groups = messages.Join(
                        users,
                        m => (string?)m.CreatedById,
                        u => (string)u.Id,
                        (Message, CreatedBy) => new
                        {
                            Message,
                            CreatedBy
                        });

        return groups.Select(g => new MessageDto(g.Message.Id, g.Message.ChannelId, g.Message.Content, g.Message.Created, new UserDto(g.CreatedBy.Id.ToString(), g.CreatedBy.Name), null, null));
    }
}