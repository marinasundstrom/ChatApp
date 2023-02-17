using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Features.Channels;

public interface ITodoNotificationService
{

}

public class TodoNotificationService : ITodoNotificationService
{
    private readonly IHubContext<ChatHub, IChatHubClient> hubsContext;

    public TodoNotificationService(IHubContext<ChatHub, IChatHubClient> hubsContext)
    {
        this.hubsContext = hubsContext;
    }
}