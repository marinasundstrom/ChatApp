using ChatApp.Features.Users;

namespace ChatApp.Features.Chat;

public sealed record MessageDto(Guid Id, Guid ChannelId, string Content, DateTimeOffset Created, UserDto CreatedBy, DateTimeOffset? LastModified, UserDto? LastModifiedBy);


public sealed record TodoDto(int Id, string Title, string? Description, TodoStatusDto Status, UserDto? AssignedTo, double? EstimatedHours, double? RemainingHours, DateTimeOffset Created, UserDto CreatedBy, DateTimeOffset? LastModified, UserDto? LastModifiedBy);

public enum TodoStatusDto
{
    NotStarted,
    InProgress,
    OnHold,
    Completed
}
