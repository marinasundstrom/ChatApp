using ChatApp.Features.Users;

namespace ChatApp.Features.Chat;

public sealed record MessageDto(Guid Id, Guid ChannelId, ReplyMessageDto? ReplyTo, string Content, DateTimeOffset Published, UserDto PublishedBy, DateTimeOffset? LastModified, UserDto? LastModifiedBy, DateTimeOffset? Deleted, UserDto? DeletedBy);

public sealed record ReplyMessageDto(Guid Id, Guid ChannelId, string Content, DateTimeOffset Published, UserDto PublishedBy, DateTimeOffset? LastModified, UserDto? LastModifiedBy, DateTimeOffset? Deleted, UserDto? DeletedBy);

public enum TodoStatusDto
{
    NotStarted,
    InProgress,
    OnHold,
    Completed
}
