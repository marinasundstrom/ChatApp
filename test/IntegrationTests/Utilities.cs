using ChatApp.Infrastructure.Persistence;

namespace ChatApp.IntegrationTests;

internal class Utilities
{
    public static Guid ChannelId { get; private set; } = Guid.NewGuid();

    public static async Task InitializeDbForTests(ApplicationDbContext db)
    {
        //db.Users.Add(new Domain.Entities.User("1234", "Test Testsson", "test@email.com"));

        db.Channels.Add(new Domain.Entities.Channel(ChannelId, "Test channel"));

        await db.SaveChangesAsync();
    }
}