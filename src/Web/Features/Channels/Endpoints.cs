﻿using Asp.Versioning.Builder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Common;
using ChatApp.Domain;
using ChatApp.Extensions;
using ChatApp.Features.Channels.Commands;
using ChatApp.Features.Channels.Queries;

namespace ChatApp.Features.Channels;

public static class Endpoints
{
    public static WebApplication MapMessageEndpoints(this WebApplication app)
    {
        var channels = app.NewVersionedApi("Messages");

        MapVersion1(channels);

        return app;
    }

    private static void MapVersion1(IVersionedEndpointRouteBuilder channels)
    {
        var group = channels.MapGroup("/v{version:apiVersion}/Messages")
            //.WithTags("Messages")
            .HasApiVersion(1, 0)
            .RequireAuthorization()
            .WithOpenApi();

          
        group.MapGet("/", GetMessages)
            .WithName($"Messages_{nameof(GetMessages)}")
            .Produces<ItemsResult<MessageDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status429TooManyRequests)
            .RequireRateLimiting("fixed");

  /*
        group.MapGet("/{id}", GetMessageById)
            .Produces<MessageDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName(nameof(GetMessageById));

        group.MapPost("/", CreateMessage)
            .Produces<MessageDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}", DeleteMessage)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id}/Title", UpdateTitle)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id}/Description", UpdateDescription)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id}/Status", UpdateStatus)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id}/AssignedUser", UpdateAssignedUser)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id}/EstimatedHours", UpdateEstimatedHours)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id}/RemainingHours", UpdateRemainingHours)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        */
    }

 

    public static async Task<ItemsResult<MessageDto>> GetMessages(int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default, IMediator mediator = default!)
        => await mediator.Send(new GetMessages(page, pageSize, sortBy, sortDirection), cancellationToken);

   /*
    public static async Task<IResult> GetMessageById(int id, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new GetMessageById(id), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> CreateMessage(CreateMessageRequest request, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new CreateMessage(request.Title, request.Description, request.Status, request.AssignedTo, request.EstimatedHours, request.RemainingHours), cancellationToken);
        return result.Handle(
            onSuccess: data => Results.CreatedAtRoute(nameof(GetMessageById), new { id = data.Id }, data),
            onError: error => Results.Problem(detail: error.Detail, title: error.Title, type: error.Id));
    }

    public static async Task<IResult> DeleteMessage(int id, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new DeleteMessage(id), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> UpdateTitle(int id, [FromBody] string title, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new UpdateTitle(id, title), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> UpdateDescription(int id, [FromBody] string? description, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new UpdateDescription(id, description), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> UpdateStatus(int id, [FromBody] MessageStatusDto status, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new UpdateStatus(id, status), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> UpdateAssignedUser(int id, [FromBody] string? userId, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new UpdateAssignedUser(id, userId), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> UpdateEstimatedHours(int id, [FromBody] double? hours, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new UpdateEstimatedHours(id, hours), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> UpdateRemainingHours(int id, [FromBody] double? hours, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new UpdateRemainingHours(id, hours), cancellationToken);
        return HandleResult(result);
    }

    private static IResult HandleResult(Result result) => result.Handle(
            onSuccess: () => Results.Ok(),
            onError: error =>
            {
                if (error.Id.EndsWith("NotFound"))
                {
                    return Results.NotFound();
                }
                return Results.Problem(detail: error.Detail, title: error.Title, type: error.Id);
            });

    private static IResult HandleResult<T>(Result<T> result) => result.Handle(
            onSuccess: data => Results.Ok(data),
            onError: error =>
            {
                if (error.Id.EndsWith("NotFound"))
                {
                    return Results.NotFound();
                }
                return Results.Problem(detail: error.Detail, title: error.Title, type: error.Id);
            });
            
            */
}