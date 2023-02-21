using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Entities;

public sealed class Message : AggregateRoot<MessageId>, IAuditable
{
    private Message() : base(new MessageId())
    {
    }

    public Message(ChannelId channelId, string text)
        : base(new MessageId())
    {
        ChannelId = channelId;
        Content = text;

        AddDomainEvent(new MessagePosted(channelId, new MessagePostedData(Id, Content)));
    }

    public Message(ChannelId channelId, MessageId replyToId, string text)
        : this(channelId, text)
    {
        ReplyToId = replyToId;
    }

    public MessageId? ReplyToId { get; set; }

    public string Content { get; private set; } = null!;

    public bool UpdateContent(string newContent) 
    {
        if(newContent == Content) 
            return false;

        Content = newContent;

        AddDomainEvent(new MessageEdited(ChannelId, new MessageEditedData(Id, Content)));

        return true;
    }

    public DateTimeOffset Published => Created;

    public ChannelId ChannelId { get; private set; }

    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }

    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public UserId? DeletedById { get; set; }
    public DateTimeOffset? Deleted { get; set; }
}
