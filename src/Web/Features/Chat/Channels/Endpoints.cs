using Asp.Versioning.Builder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Common;
using ChatApp.Domain;
using ChatApp.Extensions;

namespace ChatApp.Features.Chat.Channels;

public static class Endpoints
{
    public static WebApplication MapChannelEndpoints(this WebApplication app)
    {
        var channels = app.NewVersionedApi("Channels");

        MapVersion1(channels);

        return app;
    }

    private static void MapVersion1(IVersionedEndpointRouteBuilder channels)
    {
        var group = channels.MapGroup("/v{version:apiVersion}/Channels")
            //.WithTags("Channels")
            .HasApiVersion(1, 0)
            .RequireAuthorization()
            .WithOpenApi();

          
        group.MapGet("/", GetChannels)
            .WithName($"Channels_{nameof(GetChannels)}")
            .Produces<ItemsResult<ChannelDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status429TooManyRequests)
            .RequireRateLimiting("fixed");
    }

    public static async Task<ItemsResult<ChannelDto>> GetChannels(int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default, IMediator mediator = default!)
        => await mediator.Send(new GetChannels(page, pageSize, sortBy, sortDirection), cancellationToken);
}