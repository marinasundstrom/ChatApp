using MediatR;
using FastEndpoints;

namespace ChatApp.Features.Channels.Messages.PostMessage;

public class CreateMessageRequest
{
    public Guid ChannelId { get; set; }

    [FromBody]
    public string Content { get; set; } = default!;
}

public sealed class ApiEndpoint : Endpoint<CreateMessageRequest>
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

    public override async Task HandleAsync(CreateMessageRequest req, CancellationToken ct)
    {
        await mediator.Send(new PostMessage(req.ChannelId, req.Content));
    }
}