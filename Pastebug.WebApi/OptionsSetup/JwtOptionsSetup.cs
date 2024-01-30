using Microsoft.Extensions.Options;
using Pastebug.BLL.auth;

namespace Pastebug.WebApi.OptionsSetup;

public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    private readonly IConfiguration _configuration;
    private const string _seciontName = "Jwt";

    public JwtOptionsSetup(IConfiguration configuration) => _configuration = configuration;

    public void Configure(JwtOptions options)
    {
        _configuration.GetSection(_seciontName).Bind(options);
    }
}
