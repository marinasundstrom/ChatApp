using FluentValidation;
using MediatR;
using ChatApp.Domain;

namespace ChatApp.Features.Chat.Messages;

public record GetMessageById(Guid Id) : IRequest<Result<MessageDto>>
{
    public class Validator : AbstractValidator<GetMessageById>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<GetMessageById, Result<MessageDto>>
    {
        private readonly IMessageRepository messageRepository;

        public Handler(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public async Task<Result<MessageDto>> Handle(GetMessageById request, CancellationToken cancellationToken)
        {
            var todo = await messageRepository.FindByIdAsync(request.Id, cancellationToken);

            if (todo is null)
            {
                return Result.Failure<MessageDto>(Errors.Messages.MessageNotFound);
            }

            return Result.Success(todo.ToDto());
        }
    }
}