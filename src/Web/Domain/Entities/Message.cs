using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Entities;

public sealed class Message : AggregateRoot<MessageId>, IAuditable, ISoftDelete
{
    private Message() : base(new MessageId())
    {
    }

    public Message(ChannelId channelId, string content)
        : base(new MessageId())
    {
        ChannelId = channelId;
        Content = content;

        AddDomainEvent(new MessagePosted(ChannelId, Id, null));
    }

    public Message(ChannelId channelId, MessageId replyToId, string content)
        : base(new MessageId())
    {
        ChannelId = channelId;
        Content = content;
        
        ReplyToId = replyToId;

        AddDomainEvent(new MessagePosted(ChannelId, Id, ReplyToId));
    }

    public MessageId? ReplyToId { get; set; }

    public string Content { get; private set; } = null!;

    public bool UpdateContent(string newContent) 
    {
        if(newContent == Content) 
            return false;

        Content = newContent;

        AddDomainEvent(new MessageEdited(ChannelId, Id, Content));

        return true;
    }

    public void DeleteMarkForDeletion()
    {
        UpdateContent(string.Empty);

        AddDomainEvent(new MessageDeleted(ChannelId, Id));
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
