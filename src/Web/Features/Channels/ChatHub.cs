using ChatApp.Features.Channels.Messages.PostMessage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Features.Channels;

[Authorize]
public sealed class ChatHub : Hub<IChatHubClient>
{
    private readonly IMediator mediator;
    private readonly ICurrentUserService currentUserService;

    public ChatHub(IMediator mediator, ICurrentUserService currentUserService)
    {
        this.mediator = mediator;
        this.currentUserService = currentUserService;
    }

    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext is not null)
        {
            if (httpContext.Request.Query.TryGetValue("channelId", out var channelId))
            {
                Groups.AddToGroupAsync(this.Context.ConnectionId, $"channel-{channelId}");
            }
        }

        return base.OnConnectedAsync();
    }

    public async Task PostMessage(string channelId, string content) 
    {
        currentUserService.SetUser(Context.User!);

        await mediator.Send(new PostMessage(Guid.Parse(channelId), content));   
    }

    public async Task EditMessage(string channelId, string messageId, string content) 
    {
        currentUserService.SetUser(Context.User!);
        
        var senderId = Context.UserIdentifier!;

        await Clients
            .Group($"channel-{channelId}")
            //.GroupExcept($"channel-{channelId}", Context.ConnectionId)
            .MessageEdited(channelId, messageId, content);
    }

    public async Task DeleteMessage(string channelId, string messageId) 
    {
        currentUserService.SetUser(Context.User!);

        var senderId = Context.UserIdentifier!;

        await Clients
            .Group($"channel-{channelId}")
            //.GroupExcept($"channel-{channelId}", Context.ConnectionId)
            .MessageDeleted(channelId, messageId);
    }
}

public interface IChatHubClient
{
    Task MessagePosted(MessageDto message);

    Task MessageEdited(string channelId, string senderId, string content);

    Task MessageDeleted(string channelId, string messageId);
}