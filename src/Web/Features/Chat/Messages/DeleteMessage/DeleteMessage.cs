using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Domain;

namespace ChatApp.Features.Chat.Messages;

public sealed record DeleteMessage(Guid MessageId, string Content) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<DeleteMessage>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();

            RuleFor(x => x.Content).MaximumLength(1024);
        }
    }

    public sealed class Handler : IRequestHandler<DeleteMessage, Result>
    {
        private readonly IMessageRepository messageRepository;

        public Handler(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public async Task<Result> Handle(DeleteMessage request, CancellationToken cancellationToken)
        {
            var message = await messageRepository.FindByIdAsync(request.MessageId, cancellationToken);

             var deleted = await messageRepository
                .GetAll(new MessageWithId(request.MessageId))
                .ExecuteDeleteAsync();

            return deleted > 0 
                ? Result.Success() 
                : Result.Failure(Errors.Messages.MessageNotFound);
        }
    }
}
