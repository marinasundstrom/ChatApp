using ChatApp.Features.Channels;
using ChatApp.Features.Users;

namespace ChatApp;

public static class Mappings
{
    public static MessageDto ToDto(this Message message) => new (message.Id, message.Content, message.Published, new UserDto(message.CreatedById.ToString()!, ""), message.LastModified, message.LastModifiedById is null ? null : new UserDto(message.LastModifiedById.ToString()!, ""));

    public static UserDto ToDto(this User user) => new (user.Id, user.Name);

    public static UserInfoDto ToDto2(this User user) => new (user.Id, user.Name);
}
