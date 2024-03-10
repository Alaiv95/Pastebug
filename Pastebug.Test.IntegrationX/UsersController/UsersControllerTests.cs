using System.Net;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Pastebug.BLL.Dtos;

namespace Pastebug.Tests.Integration.UsersControllerTests;

public class UsersControllerTests : BaseTest
{
    public UsersControllerTests(PasteBugAppFactory factory) : base(factory) {}
    
    [Fact]
    public async Task m()
    {
        await Task.Delay(5000);
    }
    
    [Fact]
    public async Task RegisterUser_WithValidData_IsSuccessful()
    {
        string url = "/api/Users/Register";
        UserDto userDto = new()
        {
            Email = "test@mail.com"
        };
        var payload = JsonConvert.SerializeObject(userDto);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
    
        var response = await _client.PostAsync(url, content);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task SignIn_WithValidUser_ReturnToken()
    {
        string url = "/api/Users/SignIn";
        UserDto userDto = new()
        {
            Email = "admin@mail.ru"
        };
        
        var payload = JsonConvert.SerializeObject(userDto);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
    
        var response = await _client.PostAsync(url, content);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.ReadAsStringAsync().Should().NotBeNull();
    }
}