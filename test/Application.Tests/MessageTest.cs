using System;
using System.Linq;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Events;

namespace ChatApp.Tests;

public class MessageTest
{
    [Fact]
    public void CreateMessage()
    {
        var todo = new Message(Guid.NewGuid(), "Test");

        todo.DomainEvents.OfType<MessagePosted>().Should().ContainSingle();
    }
}
