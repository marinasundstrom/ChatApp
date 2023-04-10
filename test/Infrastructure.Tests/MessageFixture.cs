using System.Data.Common;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NSubstitute;
using ChatApp.Services;
using ChatApp;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Persistence.Interceptors;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ChatApp.Infrastructure;

public class MessageFixture : IDisposable
{
    private readonly ICurrentUserService fakeCurrentUserService;
    private readonly IDateTime fakeDateTimeService;
    private SqliteConnection connection = null!;

    public MessageFixture()
    {
        fakeCurrentUserService = Substitute.For<ICurrentUserService>();
        fakeCurrentUserService.UserId.Returns("foo");

        fakeDateTimeService = Substitute.For<IDateTime>();
        fakeDateTimeService.Now.Returns(DateTime.UtcNow);
    }

    public ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .AddInterceptors(new AuditableEntitySaveChangesInterceptor(fakeCurrentUserService, fakeDateTimeService), new OutboxSaveChangesInterceptor(fakeCurrentUserService))
           .UseSqlite(GetDbConnection())
           .Options;

        var context = new ApplicationDbContext(options);

        context.Database.EnsureCreated();

        return context;
    }

    private DbConnection GetDbConnection()
    {
        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        return connection;
    }

    public void Dispose()
    {
        connection.Close();
    }
}