namespace Pastebug.Tests.Integration;


public class BaseTest
{
    private readonly PasteBugAppFactory _appFactory;

    public BaseTest()
    {
        _appFactory = new PasteBugAppFactory();
    }

    public HttpClient Client() => _appFactory.CreateClient();
}