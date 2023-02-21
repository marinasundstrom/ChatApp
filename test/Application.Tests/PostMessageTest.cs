using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using ChatApp.Domain.Events;
using ChatApp.Features.Chat.Channels;
using ChatApp.Services;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Persistence.Interceptors;
using ChatApp.Infrastructure.Persistence.Repositories;
using ChatApp.Domain.Entities;

namespace ChatApp.Features.Chat.Messages;

public class PostMessageTest
{
    [Fact]
    public async Task PostMessage_MessagePosted()
    {
        // Arrange

        var fakeCurrentUserService = Substitute.For<ICurrentUserService>();
        fakeCurrentUserService.UserId.Returns("foo");

        var fakeDateTimeService = Substitute.For<IDateTime>();
        fakeDateTimeService.Now.Returns(DateTime.UtcNow);

        var fakeDomainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
        var fakeTodoNotificationService = Substitute.For<IChatNotificationService>();

        var adminCommandProcessor = Substitute.For<IAdminCommandProcessor>();

        using (var connection = new SqliteConnection("Data Source=:memory:"))
        {
            connection.Open();

            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .AddInterceptors(new AuditableEntitySaveChangesInterceptor(fakeCurrentUserService, fakeDateTimeService), new FakeOutboxSaveChangesInterceptor(fakeDomainEventDispatcher))
                .UseSqlite(connection)
                .Options;

            using var unitOfWork = new ApplicationDbContext(dbContextOptions);

            await unitOfWork.Database.EnsureCreatedAsync();

            unitOfWork.Users.Add(new Domain.Entities.User("foo", "Test Testsson", "test@foo.com"));

            await unitOfWork.SaveChangesAsync();

            var channelRepository = new ChannelRepository(unitOfWork);

            var channel = new Channel("myChannel");

            channelRepository.Add(channel);

            await unitOfWork.SaveChangesAsync();

            var messageRepository = new MessageRepository(unitOfWork);

            var commandHandler = new PostMessage.Handler(channelRepository, messageRepository, unitOfWork, adminCommandProcessor);

            var messages = messageRepository.GetAll();

            var initialMessageCount = await messages.CountAsync();

            string title = "test";

            // Act

            var postMessageCommand = new PostMessage(channel.Id, null, title);

            var result = await commandHandler.Handle(postMessageCommand, default);

            // Assert

            Assert.True(result.IsSuccess);

            var messageId = result.GetValue();

            messages = messageRepository.GetAll();

            var newTodoCount = messages.Count();

            newTodoCount.Should().BeGreaterThan(initialMessageCount);

            // Has Domain Event been published ?

            await fakeDomainEventDispatcher
                .Received(1)
                .Dispatch(Arg.Is<MessagePosted>(d => d.Message.MessageId == messageId));
        }
    }
}
