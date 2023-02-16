using Microsoft.EntityFrameworkCore;
using Polly;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Persistence.Repositories;

namespace ChatApp.Infrastructure;

public class TodoRepositoryTest
    : IClassFixture<TodoFixture>
{
    private readonly TodoFixture fixture;

    public TodoRepositoryTest(TodoFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task TodoShouldBeAdded()
    {
        var unitOfWork = fixture.CreateDbContext();

        unitOfWork.Users.Add(new User("foo", "Test Tesston", "test@foo.com"));

        await unitOfWork.SaveChangesAsync();

        var todoRepository = new MessageRepository(unitOfWork);

        var todo = new Message("Test", "Desc");
        todoRepository.Add(todo);

        await unitOfWork.SaveChangesAsync();

        var todo2 = await todoRepository.FindByIdAsync(todo.Id);

        todo2.Should().NotBeNull();
        todo2!.Id.Should().Be(todo.Id);
    }

    [Fact]
    public async Task AllTodosShouldBeRetrieved()
    {
        var unitOfWork = fixture.CreateDbContext();

        unitOfWork.Users.Add(new User("foo", "Test Tesston", "test@foo.com"));

        await unitOfWork.SaveChangesAsync();

        var todoRepository = new MessageRepository(unitOfWork);

        var todo = new Message("Test1", "Desc");
        todoRepository.Add(todo);

        var todo2 = new Message("Test2", "Desc");
        todoRepository.Add(todo2);

        await unitOfWork.SaveChangesAsync();

        var todos = await todoRepository.GetAll().ToListAsync();

        todos.Count.Should().Be(2);
    }
}
