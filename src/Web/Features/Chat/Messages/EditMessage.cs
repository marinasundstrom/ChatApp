using FluentValidation;
using MediatR;
using ChatApp.Domain;

namespace ChatApp.Features.Chat.Messages;

public sealed record EditMessage(Guid MessageId, string Content) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<EditMessage>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();

            RuleFor(x => x.Content).MaximumLength(1024);
        }
    }

    public sealed class Handler : IRequestHandler<EditMessage, Result>
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUnitOfWork unitOfWork;

        public Handler(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
        {
            this.messageRepository = messageRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(EditMessage request, CancellationToken cancellationToken)
        {
            var message = await messageRepository.FindByIdAsync(request.MessageId, cancellationToken);

            if (message is null)
            {
                return Result.Failure(Errors.Messages.MessageNotFound);
            }

            message.UpdateContent(request.Content);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}