using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedData(ApplicationDbContext context)
    {
        context.Channels.Add(new Channel(Guid.Parse("73b202c5-3ef1-4cd8-b1ed-04c05f47e981"), "Test"));

        await context.SaveChangesAsync();
    }
}