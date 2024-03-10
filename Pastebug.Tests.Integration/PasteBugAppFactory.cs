using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pastebug.DAL;
using Pastebug.WebApi;
using Testcontainers.PostgreSql;

namespace Pastebug.Tests.Integration;

[SetUpFixture]
public class PasteBugAppFactory : WebApplicationFactory<IntegrationTestsMarker>
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("db")
        .WithUsername("pg")
        .WithPassword("pg")
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<PastebugDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<PastebugDbContext>(options =>
            {
                options
                    .UseNpgsql(_dbContainer.GetConnectionString());
            });
        });
    }

    [OneTimeSetUp]
    public Task GlobalSetup()
    {
        return _dbContainer.StartAsync();
    }


    [OneTimeTearDown]
    public Task GlobalTeardown()
    {
        return _dbContainer.StopAsync();
    }
}