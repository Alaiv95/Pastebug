using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Pastebug.BLL;
using Pastebug.BLL.auth;
using Pastebug.BLL.Mapping;
using Pastebug.DAL;
using Pastebug.WebApi.Extentions;
using Pastebug.WebApi.OptionsSetup;
using Pastebug.WebApi.utils;
using System.Reflection;
using System.Text;

namespace Pastebug.WebApi;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AddProfile(new AssemblyMapProfile(Assembly.GetExecutingAssembly()));
            config.AddProfile(new AssemblyMapProfile(typeof(IPastebugDbContext).Assembly));
            config.AddProfile(new AssemblyMapProfile(typeof(AssemblyMapProfile).Assembly));
        });

        services.AddMemoryCache();
        services.AddInfrastructure(Configuration);
        services.AddControllers();
        services.AddSwaggerGen();

        services.Configure<JwtOptions>(Configuration.GetSection("Jwt"));

        var secretKey = Configuration.GetSection("Jwt:SecretKey").Value;
        var issuer = Configuration.GetSection("Jwt:Issuer").Value;
        var audience = Configuration.GetSection("Jwt:Audience").Value;
        var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        services.AddAuthentication(config =>
        {
            config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = signinKey,
                    ValidateIssuerSigningKey = true
                };
            });

        services.AddTransient<ICurrentUserChecker, CurrentUserChecker>();
        services.AddHttpContextAccessor();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.RoutePrefix = String.Empty;
            c.SwaggerEndpoint("swagger/v1/swagger.json", "Pastebug API");
        });

        app.UseMyExceptionHandler();
        
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
