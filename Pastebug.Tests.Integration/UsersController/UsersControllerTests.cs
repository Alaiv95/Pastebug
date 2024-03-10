using System.Net;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Pastebug.BLL.Dtos;

namespace Pastebug.Tests.Integration.UsersControllerTests;

public class UsersControllerTests : BaseTest
{
    [Test]
    public async Task RegisterUser_WithValidData_IsSuccessful()
    {
        string url = "/api/Users/Register";
        UserDto userDto = new()
        {
            Email = "test@mail.com"
        };
        var payload = JsonConvert.SerializeObject(userDto);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
    
        var response = await Client().PostAsync(url, content);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    //
    // [Test]
    // public async Task SignIn_WithValidUser_ReturnToken()
    // {
    //     string url = "/api/Users/SignIn";
    //     UserDto userDto = new()
    //     {
    //         Email = "admin@mail.ru"
    //     };
    //     
    //     var payload = JsonConvert.SerializeObject(userDto);
    //     var content = new StringContent(payload, Encoding.UTF8, "application/json");
    //
    //     var response = await Client().PostAsync(url, content);
    //     
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    //     response.Content.Should().BeOfType(typeof(string));
    // }
}