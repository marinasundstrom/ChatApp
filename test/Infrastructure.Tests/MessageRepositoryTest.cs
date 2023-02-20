using Microsoft.EntityFrameworkCore;
using Polly;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Persistence.Repositories;

namespace ChatApp.Infrastructure;

public class MessageRepositoryTest
    : IClassFixture<MessageFixture>
{
    private readonly MessageFixture fixture;

    public MessageRepositoryTest(MessageFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task MessageShouldBeAdded()
    {
        var unitOfWork = fixture.CreateDbContext();

        unitOfWork.Users.Add(new Domain.Entities.User("foo", "Test Testsson", "test@foo.com"));

        await unitOfWork.SaveChangesAsync();

        var channelRepository = new ChannelRepository(unitOfWork);

        var channel = new Channel("myChannel");

        channelRepository.Add(channel);

        await unitOfWork.SaveChangesAsync();

        var messageRepository = new MessageRepository(unitOfWork);

        var message = new Message(channel.Id, "Test 1");
        messageRepository.Add(message);

        await unitOfWork.SaveChangesAsync();

        var message2 = await messageRepository.FindByIdAsync(message.Id);

        message2.Should().NotBeNull();
        message2!.Id.Should().Be(message.Id);
    }

    [Fact]
    public async Task AllMessagesShouldBeRetrieved()
    {
        var unitOfWork = fixture.CreateDbContext();

        unitOfWork.Users.Add(new Domain.Entities.User("foo", "Test Testsson", "test@foo.com"));

        await unitOfWork.SaveChangesAsync();

        var channelRepository = new ChannelRepository(unitOfWork);

        var channel = new Channel("myChannel");

        channelRepository.Add(channel);

        await unitOfWork.SaveChangesAsync();

        var messageRepository = new MessageRepository(unitOfWork);

        var message = new Message(channel.Id, "Test 1");
        messageRepository.Add(message);

        var message2 = new Message(channel.Id, "Test 2");
        messageRepository.Add(message2);

        await unitOfWork.SaveChangesAsync();

        var messages = await messageRepository.GetAll().ToListAsync();

        messages.Count.Should().Be(2);
    }
}
