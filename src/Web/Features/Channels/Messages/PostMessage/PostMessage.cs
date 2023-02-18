using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Domain;
using FastEndpoints;

namespace ChatApp.Features.Channels.Messages.PostMessage;

public sealed record PostMessage(Guid ChannelId, string Content) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<PostMessage>
    {
        public Validator()
        {
            // RuleFor(x => x.Content).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Content).MaximumLength(1024);
        }
    }

    public sealed class Handler : IRequestHandler<PostMessage, Result>
    {
        private readonly IChannelRepository channelRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IUnitOfWork unitOfWork;

        public Handler(IChannelRepository channelRepository, IMessageRepository messageRepository, IUnitOfWork unitOfWork)
        {
            this.channelRepository = channelRepository;
            this.messageRepository = messageRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(PostMessage request, CancellationToken cancellationToken)
        {
            var hasChannel = await channelRepository
                .GetAll(new ChannelWithId(request.ChannelId))
                .AnyAsync(cancellationToken);

            if (!hasChannel)
            {
                return Result.Failure<MessageDto>(Errors.Channels.ChannelNotFound);
            }

            var message =  new Message(request.ChannelId, request.Content);

            messageRepository.Add(message);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}