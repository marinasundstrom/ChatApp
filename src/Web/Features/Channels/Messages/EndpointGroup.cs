using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Domain;
using FastEndpoints;

namespace ChatApp.Features.Channels.Messages;

public class ChannelsGroup : Group
{
    public ChannelsGroup()
    {
        Configure("channels", ep =>
        {
            ep.AllowAnonymous();
            ep.Description(x => x.Produces(400));
        });
    }
}
