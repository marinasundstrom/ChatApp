using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Domain;
using FastEndpoints;
using ChatApp.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Distributed;
namespace ChatApp.Features.Channels.Messages.PostMessage;

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
        private readonly IDistributedCache distributedCache;

        public Handler(
            IChannelRepository channelRepository, 
            IMessageRepository messageRepository, 
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IDistributedCache distributedCache)
        {
            this.channelRepository = channelRepository;
            this.messageRepository = messageRepository;
            this.unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
            this.distributedCache = distributedCache;
        }

        public async Task<Result<MessageId>> Handle(PostMessage request, CancellationToken cancellationToken)
        {
            var hasChannel = await channelRepository
                .GetAll(new ChannelWithId(request.ChannelId))
                .AnyAsync(cancellationToken);

            if (!hasChannel)
            {
                return Result.Failure<MessageId>(Errors.Channels.ChannelNotFound);
            }

            var message = new Message(request.ChannelId, request.Content);

            messageRepository.Add(message);

            var messageId = message.Id;
            var userId = currentUserService.UserId;
            var connectionId = currentUserService.ConnectionId;

            await StoreSenderConnectionId(messageId, userId!, connectionId!, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(messageId);
        }

        private async Task StoreSenderConnectionId(MessageId messageId, string userId, string connectionId, CancellationToken cancellationToken)
        {
            await distributedCache.SetAsync(messageId.ToString(), new CachedMessageSender(userId, connectionId!), new DistributedCacheEntryOptions(), cancellationToken);
        }
    }
}