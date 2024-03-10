using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pastebug.BLL.auth;
using Pastebug.BLL.Dtos;
using Pastebug.BLL.Services;
using Pastebug.BLL.Utils;
using Pastebug.BLL.Vms;
using Pastebug.DAL.Repositories;
using Pastebug.WebApi.Controllers;
using Pastebug.WebApi.utils;

namespace Pastebug.Tests.Unit.Controllers;

public class PasteControllerFindTests
{
    private ICurrentUserChecker _checker;
    private Mock<IPasteService> _pasteServiceMock;

    [SetUp]
    public void setup()
    {
        _pasteServiceMock = new Mock<IPasteService>();

        var httpMock = new Mock<IHttpContextAccessor>();
        var userRepMock = new Mock<IUserRepository>();
        var jwtMock = new Mock<IJwtToken>();
   
        _checker = new CurrentUserChecker(httpMock.Object, new UserService(userRepMock.Object, jwtMock.Object));
    }

    [Test]
    public async Task GetPaste_WithExistingId_Return200OK()
    {
        // Arrange
        PasteController pasteController = new PasteController(_pasteServiceMock.Object, _checker);
        _pasteServiceMock
            .Setup(service => service.GetPasteByHash(It.IsAny<string>()))
            .ReturnsAsync(() => new PasteVm());
        int expectedResult = StatusCodes.Status200OK;

        //Act
        OkObjectResult? result = await pasteController.GetPaste("foo") as OkObjectResult;

        //Assert
        result.Should().NotBe(null);
        result?.StatusCode.Should().Be(expectedResult);
    }

    [Test]
    public async Task GetPaste_WithNotExistingId_ReturnNoContent()
    {
        // Arrange
        _pasteServiceMock
            .Setup(service => service.GetPasteByHash(It.IsAny<string>()))
            .ReturnsAsync(() => null);

        PasteController pasteController = new PasteController(_pasteServiceMock.Object, _checker);
        int expectedResult = StatusCodes.Status404NotFound;

        //Act
        NotFoundResult? result = await pasteController.GetPaste("foo") as NotFoundResult;

        //Assert
        result.Should().NotBe(null);
        result?.StatusCode.Should().Be(expectedResult);
    }

    [Test]
    public async Task GetPaste_WithExistingId_ReturnPaste()
    {
        // Arrange
        _pasteServiceMock
            .Setup(service => service.GetPasteByHash(It.IsAny<string>()))
            .ReturnsAsync(new PasteVm());

        PasteController pasteController = new PasteController(_pasteServiceMock.Object, _checker);
        //Act
        OkObjectResult? result = await pasteController.GetPaste("bar") as OkObjectResult;
        
        //Assert
        result?.Value.Should().BeOfType<PasteVm>();
    }

    [Test]
    public async Task GetPaste_WithExistingId_ReturnPasteWithExpectedValues()
    {
        // Arrange
        HashGenerator hashGenerator = new HashGenerator();
        string value = hashGenerator.Generate();

        PasteVm paste = new PasteVm
        {
            Title = "Title",
            Text = "Text"
        };

        var mockPasteService = new Mock<IPasteService>();
        mockPasteService
            .Setup(service => service.GetPasteByHash(value))
            .ReturnsAsync(paste);


        PasteController pasteController = new PasteController(mockPasteService.Object, _checker);

        //Act
        OkObjectResult result = await pasteController.GetPaste(value) as OkObjectResult;

        //Assert
        result.Value.Should().Be(paste);
    }

    [Test]
    public async Task PasteCreate_WithValidData_Returns200ok()
    {
        // Arrange
        _pasteServiceMock
            .Setup(s => s.CreatePaste(It.IsAny<PasteDto>()))
            .ReturnsAsync("foobar");

        PasteController controller = new PasteController(_pasteServiceMock.Object, _checker);

        // Act
        OkObjectResult? result = await controller.CreatePaste(new PasteDto()) as OkObjectResult;

        // Assert
        result?.StatusCode.Should().Be(200);
    }

    [Test]
    public async Task PasteCreate_WithValidData_ReturnsString()
    {
        // Arrange
        string expectedResult = "foobar";
        _pasteServiceMock
            .Setup(s => s.CreatePaste(It.IsAny<PasteDto>()))
            .ReturnsAsync(expectedResult);

        PasteController controller = new PasteController(_pasteServiceMock.Object, _checker);

        // Act
        OkObjectResult? result = await controller.CreatePaste(new PasteDto() { Text = "foobar" }) as OkObjectResult;

        // Assert
        result?.Value.Should().Be(expectedResult);
    }

    [Test]
    public async Task PasteSearch_WithValidFilter_Return200OK()
    {
        // Arrange
        _pasteServiceMock
            .Setup(s => s.SearchPaste(It.IsAny<PasteFilterModel>(), It.IsAny<Guid>()));

        PasteController controller = new PasteController(_pasteServiceMock.Object, _checker);

        // Act
        OkObjectResult? result = await controller.SearchPaste(new PasteFilterModel { UserId = Guid.NewGuid()}) as OkObjectResult;

        // Assert
        result?.StatusCode.Should().Be(200);
    }

    [Test]
    public async Task PasteSearch_WithValidFilter_ReturnListOfPastes()
    {
        var expectedRes = new List<PasteVm>();
        // Arrange
        _pasteServiceMock
            .Setup(s => s.SearchPaste(It.IsAny<PasteFilterModel>(), It.IsAny<Guid>()))
            .ReturnsAsync(expectedRes);

        PasteController controller = new PasteController(_pasteServiceMock.Object, _checker);

        // Act
        OkObjectResult? result = await controller.SearchPaste(new PasteFilterModel { UserId = Guid.NewGuid() }) as OkObjectResult;

        // Assert
        result?.Value.Should().NotBeNull();
        result?.Value.Should().BeOfType<List<PasteVm>>();
    }

    [Test]
    public async Task PasteSearch_WithInValidFilter_Return204()
    {
        var expectedRes = new List<PasteDto>();
        // Arrange
        _pasteServiceMock
            .Setup(s => s.SearchPaste(It.IsAny<PasteFilterModel>(), It.IsAny<Guid>()))
            .ReturnsAsync(() => null);

        PasteController controller = new PasteController(_pasteServiceMock.Object, _checker);

        // Act
        NoContentResult? result = await controller.SearchPaste(new PasteFilterModel()) as NoContentResult;

        // Assert
        result?.Should().NotBeNull();
        result?.StatusCode.Should().Be(204);
    }

    [Test]
    public async Task PasteDelete_WithValidHash_Return204()
    {
        // Arrange
        PasteController controller = new PasteController(_pasteServiceMock.Object, _checker);

        // Act
        NoContentResult? result = await controller.RemovePaste("string") as NoContentResult;

        // Assert
        result?.Should().NotBeNull();
        result?.StatusCode.Should().Be(204);
    }


    [Test]
    public async Task PasteDelete_ShouldCallServiceOnce()
    {
        // Arrange
        PasteController controller = new PasteController(_pasteServiceMock.Object, _checker);

        // Act
        NoContentResult? result = await controller.RemovePaste("string") as NoContentResult;

        // Assert
        _pasteServiceMock.Verify(ps => ps.RemovePaste(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
    }
}
