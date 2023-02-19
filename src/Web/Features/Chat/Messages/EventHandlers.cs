using ChatApp.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace ChatApp.Features.Chat.Messages.EventHandlers;

public sealed class MessagePostedEventHandler : IDomainEventHandler<MessagePosted>
{
    private readonly IMessageRepository messagesRepository;
    private readonly IUserRepository userRepository;
    private readonly IChatNotificationService chatNotificationService;
    private readonly IMessageSenderCache messageSenderCache;

    public MessagePostedEventHandler(
        IMessageRepository messagesRepository, 
        IUserRepository userRepository,
        IChatNotificationService chatNotificationService,
        IMessageSenderCache messageSenderCache)
    {
        this.messagesRepository = messagesRepository;
        this.userRepository = userRepository;
        this.chatNotificationService = chatNotificationService;
        this.messageSenderCache = messageSenderCache;
    }

    public async Task Handle(MessagePosted notification, CancellationToken cancellationToken)
    {
        var message = await messagesRepository.FindByIdAsync(notification.Message.MessageId, cancellationToken);

        if (message is null)
            return;

        await SendConfirmationToSender(message, cancellationToken);

        var user = await userRepository.FindByIdAsync(message.CreatedById.GetValueOrDefault(), cancellationToken);

        if (user is null)
            return;

        await NotifyChannel(message, user, cancellationToken);

        await RemoveCachedSenderConnectionId(message);
    }

    private async Task SendConfirmationToSender(Message message, CancellationToken cancellationToken)
    {
        await chatNotificationService.SendConfirmationToSender(
            message.ChannelId.ToString(), message.Id.ToString(), cancellationToken);
    }

    private async Task NotifyChannel(Message message, User user, CancellationToken cancellationToken)
    {
        MessageDto messageDto = CreateMessageDto(message, user);

        await chatNotificationService.NotifyMessagePosted(
            messageDto, cancellationToken);
    }

    private static MessageDto CreateMessageDto(Message message, User user)
    {
        return new MessageDto(message.Id, message.ChannelId, message.Content, message.Created,
            new Users.UserDto(user.Id, user.Name),
            null, null);
    }

    private async Task RemoveCachedSenderConnectionId(Message message)
    {
        await messageSenderCache.RemoveCachedSenderConnectionId(message.Id.ToString());
    }
}
