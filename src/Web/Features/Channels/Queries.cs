using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Common;
using ChatApp.Domain;
using ChatApp.Extensions;

namespace ChatApp.Features.Channels.Queries;

public record GetMessages(int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<ItemsResult<MessageDto>>
{
    public class Handler : IRequestHandler<GetMessages, ItemsResult<MessageDto>>
    {
        private readonly IMessageRepository messageRepository;

        public Handler(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public async Task<ItemsResult<MessageDto>> Handle(GetMessages request, CancellationToken cancellationToken)
        {
            var query = messageRepository.GetAll();

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderBy(x => x.Created);
            }

            var messages = await query
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            return new ItemsResult<MessageDto>(messages.Select(x => x.ToDto()), totalCount);
        }
    }
}

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