namespace ChatApp.Domain.ValueObjects;

public readonly struct MessageId
{
    public MessageId(Guid value) => Value = value;

    public Guid Value { get; } = Guid.NewGuid();

    public override string ToString()
    {
        return Value.ToString();
    }

    public static bool operator ==(MessageId lhs, MessageId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(MessageId lhs, MessageId rhs) => lhs.Value != rhs.Value;

    public static implicit operator MessageId(Guid id) => new MessageId(id);

    public static implicit operator MessageId?(Guid? id) => id is null ? (MessageId?)null : new MessageId(id.GetValueOrDefault());

    public static implicit operator Guid(MessageId id) => id.Value;
}
