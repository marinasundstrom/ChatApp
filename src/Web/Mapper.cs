using ChatApp.Features.Channels;
using ChatApp.Features.Users;

namespace ChatApp;

public static class Mappings
{
    public static MessageDto ToDto(this Message message) => new (message.Id, message.Content, message.Published, null!, null, null);

    public static UserDto ToDto(this User user) => new (user.Id, user.Name);

    public static UserInfoDto ToDto2(this User user) => new (user.Id, user.Name);
}
