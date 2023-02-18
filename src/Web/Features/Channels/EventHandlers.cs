using ChatApp.Common;

namespace ChatApp.Features.Channels.EventHandlers;

public sealed class MessagePostedEventHandler : IDomainEventHandler<MessagePosted>
{
    private readonly IMessageRepository messagesRepository;
    private readonly IUserRepository userRepository;
    private readonly IChatNotificationService chatNotificationService;

    public MessagePostedEventHandler(IMessageRepository messagesRepository, IUserRepository userRepository, IChatNotificationService chatNotificationService)
    {
        this.messagesRepository = messagesRepository;
        this.userRepository = userRepository;
        this.chatNotificationService = chatNotificationService;
    }

    public async Task Handle(MessagePosted notification, CancellationToken cancellationToken)
    {
        var message = await messagesRepository.FindByIdAsync(notification.Message.MessageId, cancellationToken);

        if (message is null)
            return;

        var user = await userRepository.FindByIdAsync(message.CreatedById.GetValueOrDefault(), cancellationToken);

        if (user is null)
            return;

        var messageDto = new MessageDto(message.Id, message.ChannelId, message.Content, message.Created, 
            new Users.UserDto(user.Id, user.Name),
            null, null);

        await chatNotificationService.MessagePosted(messageDto, cancellationToken);
    }
}
