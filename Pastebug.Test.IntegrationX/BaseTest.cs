namespace Pastebug.Tests.Integration;


public class BaseTest : IClassFixture<PasteBugAppFactory>
{
    protected readonly HttpClient _client;

    public BaseTest(PasteBugAppFactory appFactory)
    {
        _client = appFactory.CreateClient();
    }
}