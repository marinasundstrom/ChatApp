using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Events;

public sealed record MessagePosted(ChannelId ChannelId, MessageId MessageId, MessageId? ReplyToId) : DomainEvent;

public sealed record MessageEdited(ChannelId ChannelId, MessageId MessageId, string Content) : DomainEvent;

public sealed record MessageDeleted(ChannelId ChannelId, MessageId MessageId) : DomainEvent;
