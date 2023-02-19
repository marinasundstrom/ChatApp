using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Domain;
using FastEndpoints;
using ChatApp.Domain.ValueObjects;

using Microsoft.Extensions.Caching.Distributed;
namespace ChatApp.Features.Chat.Messages;

public sealed record PostMessage(Guid ChannelId, string Content) : IRequest<Result<MessageId>>
{
    public sealed class Validator : AbstractValidator<PostMessage>
    {
        public Validator()
        {
            // RuleFor(x => x.Content).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Content).MaximumLength(1024);
        }
    }

    public sealed class Handler : IRequestHandler<PostMessage, Result<MessageId>>
    {
        private readonly IChannelRepository channelRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUserService currentUserService;
        private readonly IChatNotificationService chatNotificationService;
        private readonly IMessageSenderCache messageSenderCache;

        public Handler(
            IChannelRepository channelRepository,
            IMessageRepository messageRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,     
            IChatNotificationService chatNotificationService,
            IMessageSenderCache messageSenderCache)
        {
            this.channelRepository = channelRepository;
            this.messageRepository = messageRepository;
            this.unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
            this.chatNotificationService = chatNotificationService;
            this.messageSenderCache = messageSenderCache;
        }

        public async Task<Result<MessageId>> Handle(PostMessage request, CancellationToken cancellationToken)
        {
            var notification = request;

            var hasChannel = await channelRepository
                .GetAll(new ChannelWithId(request.ChannelId))
                .AnyAsync(cancellationToken);

            if (!hasChannel)
            {
                return Result.Failure<MessageId>(Errors.Channels.ChannelNotFound);
            }

            if (IsAdminCommand(notification.Content, out var args))
            {
                return await ProcessAdminCommand(request.ChannelId.ToString(), args, cancellationToken);
            }

            var message = new Message(request.ChannelId, request.Content);

            messageRepository.Add(message);

            await CacheSenderConnectionId(message, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(message.Id);
        }

        private async Task CacheSenderConnectionId(Message message, CancellationToken cancellationToken)
        {
            var messageId = message.Id;
            var userId = currentUserService.UserId;
            var connectionId = currentUserService.ConnectionId;

            await messageSenderCache.StoreSenderConnectionId(
                messageId.ToString(), userId!, connectionId!, cancellationToken);
        }

        private bool IsAdminCommand(string message, out string[] args)
        {
            var args0 = message.Split(' ');

            if (args0.Any() && args0[0].Equals("/admin"))
            {
                args = args0;
                return true;
            }

            args = Array.Empty<string>();
            return false;
        }

         private async Task<Result<MessageId>> ProcessAdminCommand(string channelId, string[] args, CancellationToken cancellationToken)
        {
            if (args.Length >= 2 && args[1].Equals("getNumberPosts"))
            {
                string content;

                if(args.Length == 3 && args[2].Equals("/channel")) 
                {
                    var numberOfPosts = await messageRepository.GetAll(new MessagesInChannel(Guid.Parse(channelId))).CountAsync(cancellationToken);

                    content = $"Number of posts in channel: {numberOfPosts}";
                }
                else 
                {
                    var numberOfPosts = await messageRepository.GetAll().CountAsync(cancellationToken);

                    content = $"Total number of posts: {numberOfPosts}";
                }

                MessageDto messageDto = CreateMessage(channelId, content);

                await SendMessage(messageDto, cancellationToken);
            }
            else if (args.Length > 1)
            {
                var content = $"Unknown command";

                MessageDto messageDto = CreateMessage(channelId, content);

                await SendMessage(messageDto, cancellationToken);
            }
            else
            {
                var content = $"Command expected";

                MessageDto messageDto = CreateMessage(channelId, content);

                await SendMessage(messageDto, cancellationToken);
            }

            return Result.Success(new MessageId());
        }

        private static MessageDto CreateMessage(string channelId, string content)
        {
            return new MessageDto(Guid.NewGuid(), Guid.Parse(channelId), content, DateTimeOffset.UtcNow, new Users.UserDto("system", "System"), null, null);
        }

        private async Task SendMessage(MessageDto messageDto, CancellationToken cancellationToken)
        {
            await chatNotificationService.SendMessageToUser(
                currentUserService.UserId!.ToString(), messageDto, cancellationToken);
        }
    }
}