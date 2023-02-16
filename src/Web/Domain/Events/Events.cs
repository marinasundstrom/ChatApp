using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Events;

public sealed record MessagePosted(ChannelId ChannelId, MessagePostedData Message) : DomainEvent;

public sealed record MessagePostedData(MessageId MessageId, string Content);

public sealed record MessageEdited(ChannelId ChannelId, MessageEditedData Message) : DomainEvent;

public sealed record MessageEditedData(MessageId MessageId, string Content);

public sealed record MessageDeleted(ChannelId ChannelId, MessageId MessageId) : DomainEvent;
