using ChatApp.Domain.ValueObjects;
using ChatApp.Features.Chat.Messages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Features.Chat;

[Authorize]
public sealed class ChatHub : Hub<IChatHubClient>
{
    private readonly IMediator mediator;
    private readonly ICurrentUserServiceInternal currentUserService;

    public ChatHub(IMediator mediator, ICurrentUserServiceInternal currentUserService)
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

    public async Task<Guid> PostMessage(string channelId, string? replyTo, string content) 
    {
        currentUserService.SetUser(Context.User!);
        currentUserService.SetConnectionId(Context.ConnectionId);

        return (MessageId)await mediator.Send(
            new PostMessage(Guid.Parse(channelId), replyTo is null ? null : Guid.Parse(replyTo), content));   
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

    Task MessagePostedConfirmed(string messageId);

    Task MessageEdited(string channelId, string senderId, string content);

    Task MessageDeleted(string channelId, string messageId);
}