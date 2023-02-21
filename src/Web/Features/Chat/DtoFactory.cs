using ChatApp.Features.Users;

namespace ChatApp.Features.Chat;

public interface IDtoFactory
{
    ReplyMessageDto CreateReplyMessageDto(Message replyMessage, User replyMessagePublishedBy, User? replyMessageEditedBy, User? replyMessageDeletedBy);
    MessageDto CreateMessageDto(Message message, User publishedBy, User? editedBy, User? deletedBy, ReplyMessageDto? replyMessageDto);
    UserDto CreateUserDto(User user);
}

public sealed class DtoFactory : IDtoFactory
{
    public ReplyMessageDto CreateReplyMessageDto(Message replyMessage, User replyMessagePublishedBy, User? replyMessageEditedBy, User? replyMessageDeletedBy)
    {
        return new ReplyMessageDto(
            (Guid)replyMessage.Id,
            replyMessage.ChannelId,
            replyMessage.Content,
            replyMessage.Created, CreateUserDto(replyMessagePublishedBy),
            replyMessage.LastModified, replyMessage.LastModifiedById is null ? null : CreateUserDto(replyMessageEditedBy!),
            replyMessage.Deleted, replyMessage.DeletedById is null ? null : CreateUserDto(replyMessageDeletedBy!));
    }

    public MessageDto CreateMessageDto(Message message, User publishedBy, User? editedBy, User? deletedBy, ReplyMessageDto? replyMessageDto)
    {
        return new MessageDto(
            message.Id,
            message.ChannelId,
            replyMessageDto,
            message.Content,
            message.Created, CreateUserDto(publishedBy),
            message.LastModified, message.LastModifiedById is null ? null : CreateUserDto(editedBy!),
            message.Deleted, message.DeletedById is null ? null : CreateUserDto(deletedBy!));
    }

    public UserDto CreateUserDto(User user)
    {
        return new UserDto(user!.Id.ToString(), user.Name);
    }
}
