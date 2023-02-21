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

    private static MessageDto CreateMessageDto(Message message,  User user)
    {
        return new MessageDto(message.Id, message.ChannelId, 
            message.ReplyToId is null ? null : new ReplyMessageDto(
                message.ReplyToId.GetValueOrDefault(), Guid.NewGuid(), string.Empty, DateTimeOffset.Now, new Users.UserDto(string.Empty, string.Empty), null, null, null, null), message.Content, message.Created,
            new Users.UserDto(user.Id, user.Name),
            message.LastModified, null,
            message.Deleted, null);
    }
}

public sealed class MessageEditedEventHandler : IDomainEventHandler<MessageEdited>
{
    private readonly IMessageRepository messagesRepository;
    private readonly IUserRepository userRepository;
    private readonly IChatNotificationService chatNotificationService;

    public MessageEditedEventHandler(
        IMessageRepository messagesRepository, 
        IUserRepository userRepository,
        IChatNotificationService chatNotificationService)
    {
        this.messagesRepository = messagesRepository;
        this.userRepository = userRepository;
        this.chatNotificationService = chatNotificationService;
    }

    public async Task Handle(MessageEdited notification, CancellationToken cancellationToken)
    {
        await NotifyChannelMessageEdited(notification.ChannelId.ToString(), notification.Message.MessageId.ToString(), notification.Message.Content, cancellationToken);
    }

    private async Task NotifyChannelMessageEdited(string channelId, string messageId, string content, CancellationToken cancellationToken)
    {
        await chatNotificationService.NotifyMessageEdited(
            channelId, messageId, content, cancellationToken);
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
        await NotifyChannelMessageDeleted(
            notification.ChannelId.ToString(), notification.MessageId.ToString(), cancellationToken);
    }

    private async Task NotifyChannelMessageDeleted(string channelId, string messageId, CancellationToken cancellationToken)
    {
        await chatNotificationService.NotifyMessageDeleted(
            channelId, messageId, cancellationToken);
    }
}
