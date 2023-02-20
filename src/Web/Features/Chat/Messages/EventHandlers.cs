using ChatApp.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace ChatApp.Features.Chat.Messages.EventHandlers;

public sealed class MessagePostedEventHandler : IDomainEventHandler<MessagePosted>
{
    private readonly IMessageRepository messagesRepository;
    private readonly IUserRepository userRepository;
    private readonly IChatNotificationService chatNotificationService;

    public MessagePostedEventHandler(
        IMessageRepository messagesRepository, 
        IUserRepository userRepository,
        IChatNotificationService chatNotificationService)
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

        await SendConfirmationToSender(message, cancellationToken);

        var user = await userRepository.FindByIdAsync(message.CreatedById.GetValueOrDefault(), cancellationToken);

        if (user is null)
            return;

        await NotifyChannel(message, user, cancellationToken);
    }

    private async Task SendConfirmationToSender(Message message, CancellationToken cancellationToken)
    {
        await chatNotificationService.SendConfirmationToSender(
            message.ChannelId.ToString(), message.CreatedById.GetValueOrDefault().ToString(), message.Id.ToString(), cancellationToken);
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
}


public sealed class MessageDeletedEventHandler : IDomainEventHandler<MessageDeleted>
{
    private readonly IMessageRepository messagesRepository;
    private readonly IUserRepository userRepository;
    private readonly IChatNotificationService chatNotificationService;

    public MessageDeletedEventHandler(
        IMessageRepository messagesRepository, 
        IUserRepository userRepository,
        IChatNotificationService chatNotificationService)
    {
        this.messagesRepository = messagesRepository;
        this.userRepository = userRepository;
        this.chatNotificationService = chatNotificationService;
    }

    public async Task Handle(MessageDeleted notification, CancellationToken cancellationToken)
    {
        await NotifyChannelMessageDeleted(notification.ChannelId.ToString(), notification.MessageId.ToString(), cancellationToken);
    }

    private async Task NotifyChannelMessageDeleted(string channelId, string messageId, CancellationToken cancellationToken)
    {
        await chatNotificationService.NotifyMessageDeleted(
            channelId, messageId, cancellationToken);
    }
}
