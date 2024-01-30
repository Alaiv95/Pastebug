using Pastebug.DAL;

namespace Pastebug.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var app = CreateHostBulder(args).Build();

        using(var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<PastebugDbContext>();
                context.Database.EnsureCreated();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        app.Run();
    }

    private static IHostBuilder CreateHostBulder(string[] args) =>
        Host.CreateDefaultBuilder(args)
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.UseStartup<Startup>();
             });
}
