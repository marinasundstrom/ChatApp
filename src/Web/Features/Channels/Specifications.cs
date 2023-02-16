using ChatApp.Domain.Specifications;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Features.Channels;

public class ChannelWithId : BaseSpecification<Channel>
{
    public ChannelWithId(ChannelId channelId)
    {
        Criteria = channel => channel.Id == channelId;
    }
}

public class MessageWithId : BaseSpecification<Message>
{
    public MessageWithId(MessageId messageId)
    {
        Criteria = message => message.Id == messageId;
    }
}

public class MessagesInChannel : BaseSpecification<Message>
{
    public MessagesInChannel(ChannelId channelId)
    {
        Criteria = message => message.ChannelId == channelId;
    }
}
