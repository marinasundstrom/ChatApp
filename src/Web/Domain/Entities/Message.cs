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

        // Todo: Emit Domain Event
    }

    public string Content { get; private set; } = null!;

    public bool UpdateContent(string newContent) 
    {
        if(newContent == Content) 
            return false;

        Content = newContent;

        // Todo: Emit Domain Event

        return true;
    }

    public DateTimeOffset Published => Created;

    public ChannelId ChannelId { get; private set; }

    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }

    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
