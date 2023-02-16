namespace ChatApp.Domain.ValueObjects;

public readonly struct ChannelId
{
    public ChannelId(Guid value) => Value = value;

    public Guid Value { get; } = Guid.NewGuid();

    public override string ToString()
    {
        return Value.ToString();
    }

    public static bool operator ==(ChannelId lhs, ChannelId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(ChannelId lhs, ChannelId rhs) => lhs.Value != rhs.Value;

    public static implicit operator ChannelId(Guid id) => new ChannelId(id);

    public static implicit operator ChannelId?(Guid? id) => id is null ? (ChannelId?)null : new ChannelId(id.GetValueOrDefault());

    public static implicit operator Guid(ChannelId id) => id.Value;
}
