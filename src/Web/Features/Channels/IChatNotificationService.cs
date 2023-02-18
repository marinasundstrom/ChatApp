using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Features.Channels;

public interface IChatNotificationService
{
    Task MessagePosted(MessageDto message);
    Task MessageEdited(string channelId, string messageId, string content);
    Task MessageDeleted(string channelId, string messageId);
}

public class ChatNotificationService : IChatNotificationService
{
    private readonly IHubContext<ChatHub, IChatHubClient> hubsContext;

    public ChatNotificationService(IHubContext<ChatHub, IChatHubClient> hubsContext)
    {
        this.hubsContext = hubsContext;
    }

    public async Task MessagePosted(MessageDto message) 
    {
        await hubsContext.Clients
            .Group($"channel-{message.ChannelId}")
            //.GroupExcept($"channel-{channelId}", Context.ConnectionId)
            .MessagePosted(message);
    }

    public async Task MessageEdited(string channelId, string messageId, string content) 
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            //.GroupExcept($"channel-{channelId}", Context.ConnectionId)
            .MessageEdited(channelId, messageId, content);
    }

    public async Task MessageDeleted(string channelId, string messageId) 
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            //.GroupExcept($"channel-{channelId}", Context.ConnectionId)
            .MessageDeleted(channelId, messageId);
    }
}