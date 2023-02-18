﻿using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Common;
using ChatApp.Domain;
using ChatApp.Extensions;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Features.Users;

namespace ChatApp.Features.Channels.Queries;

public record GetMessages(int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<ItemsResult<MessageDto>>
{
    public class Handler : IRequestHandler<GetMessages, ItemsResult<MessageDto>>
    {
        private readonly IMessageRepository messageRepository;
        private readonly ApplicationDbContext context;

        public Handler(IMessageRepository messageRepository, ApplicationDbContext context)
        {
            this.messageRepository = messageRepository;
            this.context = context;
        }

        public async Task<ItemsResult<MessageDto>> Handle(GetMessages request, CancellationToken cancellationToken)
        {
            var query = context.Messages.AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderByDescending(x => x.Created);
            }

            var messages = await query
                .AsNoTracking()
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            IEnumerable<MessageDto> dtos = Extensions.GetMessageDtos(context.Users, messages);

            return new ItemsResult<MessageDto>(dtos!, totalCount);
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