using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Entities;

public sealed class Channel : AggregateRoot<ChannelId>, IAuditable
{
    private Channel() : base(new ChannelId())
    {
    }

    public Channel(string title)
        : base(new ChannelId())
    {
        Title = title;

        // Todo: Emit Domain Event
    }

    internal Channel(ChannelId id, string title)
        : base(id)
    {
        Title = title;
    }

    public string Title { get; private set; } = null!;

    public bool Rename(string newTitle) 
    {
        if(newTitle == Title) 
            return false;

        Title = newTitle;

        // Todo: Emit Domain Event

        return true;
    }

    HashSet<ChannelParticipant> _participants = new HashSet<ChannelParticipant>();

    public IReadOnlyCollection<ChannelParticipant> Participants => _participants;

    public bool AddParticipant(UserId userId) 
    {
        var participant = Participants.First(x => x.UserId == userId);

        if(participant is not null) return false;

        _participants.Add(new ChannelParticipant(userId, DateTimeOffset.UtcNow));

        // Todo: Emit Domain Event

        return true;
    }

    public bool RemoveParticipant(UserId userId) 
    {
        var participant = Participants.First(x => x.UserId == userId);

        if(participant is null) return false;

        participant.Left = DateTimeOffset.UtcNow;
        //_participants.Remove(participant);

        // Todo: Emit Domain Event

        return true;
    }

    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }

    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}

public class ChannelParticipant
{
    private ChannelParticipant()
    {
    }

    public ChannelParticipant(UserId userId, DateTimeOffset joined)
    {
        UserId = userId;
        Joined = joined;
    }

    public UserId UserId { get; set; }

    public DateTimeOffset Joined { get; set; }

    public DateTimeOffset? Left { get; set; }
}