using MediatR;
using FastEndpoints;

namespace ChatApp.Features.Chat.Messages;

public sealed class ApiEndpoint : Endpoint<PostMessageRequest>
{
    private readonly IMediator mediator;

    public ApiEndpoint(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public override void Configure()
    {
        Group<ChannelsGroup>();
        Get("/{channelId}");
        Version(2);
    }

    public override async Task HandleAsync(PostMessageRequest req, CancellationToken ct)
    {
        await mediator.Send(new PostMessage(req.ChannelId, req.Content));
    }
}