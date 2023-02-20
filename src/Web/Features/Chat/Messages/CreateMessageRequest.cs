using FastEndpoints;

namespace ChatApp.Features.Chat.Messages;

public class PostMessageRequest
{
    public Guid ChannelId { get; set; }

    [FromBody]
    public string Content { get; set; } = default!;
}
