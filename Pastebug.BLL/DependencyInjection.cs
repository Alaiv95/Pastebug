using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pastebug.BLL.auth;
using Pastebug.BLL.Services;
using Pastebug.BLL.Specs;
using Pastebug.BLL.Utils;
using Pastebug.DAL;
using Pastebug.DAL.Repositories;

namespace Pastebug.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var mainDbConnection = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<PastebugDbContext>(options => options.UseSqlite(mainDbConnection));
        services.AddScoped<IPastebugDbContext>(provider => provider.GetService<PastebugDbContext>());

        // services
        services.AddTransient<IPasteService, PasteService>();
        services.AddTransient<IUserService, UserService>();

        // repositories
        services.AddTransient<PasteRepository>();
        services.AddTransient<IPasteRepository, PasteCacheRepository>();
        services.AddTransient<IUserRepository, UserRepository>();

        // specs
        services.AddTransient<IPasteSpecifications, PasteSpecifications>();

        // utils
        services.AddSingleton<IHashGenerator, HashGenerator>();
        services.AddTransient<IJwtToken, JwtToken>();

        return services;
    }
}
