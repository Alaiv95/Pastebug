﻿using Microsoft.AspNetCore.Hosting;
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
            services.RemoveDbContext<PastebugDbContext>();

            services.AddDbContext<PastebugDbContext>(options =>
            {
                options
                    .UseNpgsql(_dbContainer.GetConnectionString());
            });
            
            services.EnsureDbCreated<PastebugDbContext>();
        });
    }

    [OneTimeSetUp]
    public void Setup()
    {
        SetupAsync().Wait();
    }
    
    private async Task SetupAsync()
    {
        await _dbContainer.StartAsync();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _dbContainer.DisposeAsync();
    }
}


public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
    {
        var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<T>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }

    public static void EnsureDbCreated<T>(this IServiceCollection services) where T : DbContext
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var context = serviceProvider.GetRequiredService<T>();
        context.Database.EnsureCreated();
    }
}