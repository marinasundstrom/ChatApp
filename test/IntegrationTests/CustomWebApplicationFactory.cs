using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using ChatApp;
using ChatApp.Features.Chat.Channels;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Persistence.Interceptors;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ChatApp.Features.Chat;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ChatApp.IntegrationTests;

public sealed class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup>, IAsyncLifetime where TStartup : class
{
    static readonly TestcontainersContainer testContainer =
        new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("redis:latest")
        .WithPortBinding(6379, 6379)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
        .Build();

    public async Task InitializeAsync()
    {
        await testContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test", options => { });
        });

        builder.ConfigureServices(async services =>
        {
            var descriptor = services.Single(
        d => d.ServiceType ==
            typeof(DbContextOptions<ApplicationDbContext>));

            services.Remove(descriptor);

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlite($"Data Source=testdb.db");

                options.AddInterceptors(
                    sp.GetRequiredService<OutboxSaveChangesInterceptor>(),
                    sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

#if DEBUG
                options
                    .LogTo(Console.WriteLine)
                    .EnableSensitiveDataLogging();
#endif
            });

            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddMessageConsumers();
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                try
                {
                    await Utilities.InitializeDbForTests(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the " +
                        "database with test messages. Error: {Message}", ex.Message);
                }
            }
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await testContainer.StopAsync();
    }
}