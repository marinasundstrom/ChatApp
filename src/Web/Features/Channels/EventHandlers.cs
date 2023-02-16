using ChatApp.Common;

namespace ChatApp.Features.Channels.EventHandlers;

public sealed class MessagePostedEventHandler : IDomainEventHandler<MessagePosted>
{
    private readonly IMessageRepository todoRepository;
    private readonly IEmailService emailService;
    private readonly ITodoNotificationService todoNotificationService;

    public MessagePostedEventHandler(IMessageRepository todoRepository, IEmailService emailService, ITodoNotificationService todoNotificationService)
    {
        this.todoRepository = todoRepository;
        this.emailService = emailService;
        this.todoNotificationService = todoNotificationService;
    }

    public async Task Handle(MessagePosted notification, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.FindByIdAsync(notification.Message.MessageId, cancellationToken);

        if (todo is null)
            return;
    }
}
