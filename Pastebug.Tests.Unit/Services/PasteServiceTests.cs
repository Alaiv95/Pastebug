using Pastebug.Domain.Entities;
using Pastebug.BLL.Services;
using Moq;
using Pastebug.DAL.Repositories;
using FluentAssertions;
using Pastebug.BLL.Utils;
using Pastebug.BLL.Dtos;
using Pastebug.BLL.Specs;
using System.Linq.Expressions;
using Pastebug.BLL.Exceptions;
using AutoMapper;
using Pastebug.BLL.Vms;

namespace Pastebug.Tests.Unit.Services;

internal class PasteServiceTests
{
    private Mock<IHashGenerator> _hashGenerator;
    private Mock<IPasteSpecifications> _specifications;
    private IMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<PasteDto, Paste>());
        _mapper = new Mapper(config);

        _hashGenerator = new Mock<IHashGenerator>();
        _specifications = new Mock<IPasteSpecifications>();
    }

    [Test]
    public async Task GetById_WithExistingId_ReturnPaste()
    {
        //Arrange
        var config = new MapperConfiguration(cfg => cfg.CreateMap<Paste, PasteVm>());
        _mapper = new Mapper(config);

        var PasteRepositoryMock = new Mock<IPasteRepository>();
        PasteRepositoryMock
            .Setup(repository => repository.FindAsync(It.IsAny<string>()))
            .ReturnsAsync(() => new Paste());

        _hashGenerator
            .Setup(gen => gen.Generate())
            .Returns("123");

        PasteService pasteService = new PasteService(PasteRepositoryMock.Object, _hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        var paste = await pasteService.GetPasteByHash("foo");

        //Assert
        paste.Should().NotBeNull();
        paste.Should().BeOfType<PasteVm>();
    }

    [Test]
    public async Task GetById_WithNotExistingId_ReturnNull()
    {
        //Arrange
        var PasteRepositoryMock = new Mock<IPasteRepository>();
        PasteRepositoryMock
            .Setup(repository => repository.FindAsync(It.IsAny<string>()))
            .ReturnsAsync(() => null);

        _hashGenerator
            .Setup(gen => gen.Generate())
            .Returns("123");

        PasteService pasteService = new PasteService(PasteRepositoryMock.Object, _hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        var paste = await pasteService.GetPasteByHash("bar");

        //Assert
        paste.Should().BeNull();
    }

    [Test]
    public async Task CreatePaste_WithValidData_ReturnString()
    {
        //Arrange
        var PasteRepositoryMock = new Mock<IPasteRepository>();
        PasteRepositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<Paste>()));

        _hashGenerator
            .Setup(gen => gen.Generate())
            .Returns("123");

        PasteService pasteService = new PasteService(PasteRepositoryMock.Object, _hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        var paste = await pasteService.CreatePaste(new PasteDto());

        //Assert
        paste.Should().BeOfType(typeof(string));
    }


    [Test]
    public async Task CreatePaste_WithValidData_ReturnHashCode()
    {
        //Arrange
        string hash = "QWERTYUI";
        var hashGenerator = new Mock<IHashGenerator>();
        hashGenerator
            .Setup(gen => gen.Generate())
            .Returns(hash);

        var PasteRepositoryMock = new Mock<IPasteRepository>();
        PasteRepositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<Paste>()));

        PasteService pasteService = new PasteService(PasteRepositoryMock.Object, hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        var paste = await pasteService.CreatePaste(new PasteDto());

        //Assert
        paste.Should().Be(hash);
    }

    [Test]
    public async Task CreatePaste_WithNullPasteDto_Throws()
    {
        //Arrange
        var hashGenerator = new Mock<IHashGenerator>();
        var pasteRepositoryMock = new Mock<IPasteRepository>();

        PasteService pasteService = new PasteService(pasteRepositoryMock.Object, hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        //Assert
        Assert.ThrowsAsync<ApiException>(async () => await pasteService.CreatePaste(null));
    }

    [Test]
    public async Task CreatePaste_ShouldCallCreatePasteAsyncOnce()
    {
        //Arrange
        var hashGenerator = new Mock<IHashGenerator>();
        var pasteRepositoryMock = new Mock<IPasteRepository>();

        PasteService pasteService = new PasteService(pasteRepositoryMock.Object, hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        var result = await pasteService.CreatePaste(new PasteDto());

        //Assert
        pasteRepositoryMock.Verify(ps => ps.CreateAsync(It.IsAny<Paste>()), Times.Once);
    }

    [Test]
    public async Task SearchPaste_ReturnNull_WithNullFilter()
    {
        //Arrange
        var hashGenerator = new Mock<IHashGenerator>();
        var pasteRepositoryMock = new Mock<IPasteRepository>();

        PasteService pasteService = new PasteService(pasteRepositoryMock.Object, hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        var result = await pasteService.SearchPaste(null, Guid.Empty);

        //Assert
        result.Should().BeNull();
    }
    [Test]
    public async Task SearchPaste_ReturnPublicPastes_WhenSearchedAsOtherUser()
    {
        //Arrange
        var config = new MapperConfiguration(cfg => cfg.CreateMap<Paste, PasteVm>());
        _mapper = new Mapper(config);

        Guid searchedUserId = Guid.NewGuid();
        Guid otherUserId = Guid.NewGuid();

        List<Paste> pastes = new List<Paste>
        {
            new()
            {
                Title = "random1",
                UserId = otherUserId,
                Visibility = 1
            },
            new()
            {
                Title = "random2",
                UserId = otherUserId,
                Visibility = 2
            }
        };

        PasteFilterModel filterModel = new PasteFilterModel()
        {
            UserId = otherUserId
        };

        var hashGenerator = new Mock<IHashGenerator>();
        var pasteRepositoryMock = new Mock<IPasteRepository>();

        pasteRepositoryMock
            .Setup(repository => repository.SearchAsync(It.IsAny<Expression<Func<Paste, bool>>>()))
            .ReturnsAsync(pastes);


        PasteService pasteService = new PasteService(pasteRepositoryMock.Object, hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        var result = await pasteService.SearchPaste(filterModel, searchedUserId);

        //Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(1);

        var filteredRes = result.Where(p => p.Visibility == 1);

        result.Should().BeEquivalentTo(result);
    }

    [Test]
    public async Task SearchPaste_ReturnsEmptyList_WhenResultNull()
    {
        //Arrange
        var config = new MapperConfiguration(cfg => cfg.CreateMap<Paste, PasteVm>());
        _mapper = new Mapper(config);

        PasteFilterModel filterModel = new PasteFilterModel();

        var hashGenerator = new Mock<IHashGenerator>();
        var pasteRepositoryMock = new Mock<IPasteRepository>();

        pasteRepositoryMock
            .Setup(repository => repository.SearchAsync(It.IsAny<Expression<Func<Paste, bool>>>()))
            .ReturnsAsync(() => null);


        PasteService pasteService = new PasteService(pasteRepositoryMock.Object, hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        var result = await pasteService.SearchPaste(filterModel, Guid.Empty);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<List<PasteVm>>();
    }

    [Test]
    public async Task SearchPaste_ReturnsAll_WhenUserSameAsFilterId()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.CreateMap<Paste, PasteVm>());
        _mapper = new Mapper(config);

        Guid searchedUserId = Guid.NewGuid();

        List<Paste> pastes = new List<Paste>
        {
            new()
            {
                Title = "random1",
                UserId = searchedUserId,
                Visibility = 1
            },
            new()
            {
                Title = "random2",
                UserId = searchedUserId,
                Visibility = 2
            }
        };

        PasteFilterModel filterModel = new PasteFilterModel()
        {
            UserId = searchedUserId
        };

        var hashGenerator = new Mock<IHashGenerator>();
        var pasteRepositoryMock = new Mock<IPasteRepository>();

        pasteRepositoryMock
            .Setup(repository => repository.SearchAsync(It.IsAny<Expression<Func<Paste, bool>>>()))
            .ReturnsAsync(pastes);


        PasteService pasteService = new PasteService(pasteRepositoryMock.Object, hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        var result = await pasteService.SearchPaste(filterModel, searchedUserId);

        //Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(2);
    }

    [Test]
    public async Task PasteRemove_WithValidHash_ShouldCallRepositoryDelete()
    {
        //Arrange
        Guid userId = Guid.NewGuid();

        Paste paste = new()
        {
            Title = "LALALA",
            UserId = userId
        };

        var hashGenerator = new Mock<IHashGenerator>();
        var pasteRepositoryMock = new Mock<IPasteRepository>();

        pasteRepositoryMock
            .Setup(repository => repository.FindAsync(It.IsAny<string>()))
            .ReturnsAsync(paste);

        pasteRepositoryMock
            .Setup(repository => repository.Delete(It.IsAny<Paste>()));

        PasteService pasteService = new PasteService(pasteRepositoryMock.Object, hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        await pasteService.RemovePaste("lalala", userId);

        //Assert
        pasteRepositoryMock.Verify(ps => ps.Delete(It.IsAny<Paste>()), Times.Once);
    }

    [Test]
    public async Task PasteRemove_AsNotCreatedUser_ShouldThrow()
    {
        //Arrange
        Guid userId = Guid.NewGuid();
        Guid otherUserId = Guid.NewGuid();

        Paste paste = new()
        {
            Title = "LALALA",
            UserId = userId
        };

        var hashGenerator = new Mock<IHashGenerator>();
        var pasteRepositoryMock = new Mock<IPasteRepository>();

        pasteRepositoryMock
            .Setup(repository => repository.FindAsync(It.IsAny<string>()))
            .ReturnsAsync(paste);

        PasteService pasteService = new PasteService(pasteRepositoryMock.Object, hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        //Assert
        Assert.ThrowsAsync<ApiException>(async () => await pasteService.RemovePaste("lalala", otherUserId));
    }

    [Test]
    public async Task PasteRemove_PasteNotFound_ShouldThrow()
    {
        //Arrange
        var hashGenerator = new Mock<IHashGenerator>();
        var pasteRepositoryMock = new Mock<IPasteRepository>();

        pasteRepositoryMock
            .Setup(repository => repository.FindAsync(It.IsAny<string>()))
            .ReturnsAsync(() => null);

        PasteService pasteService = new PasteService(pasteRepositoryMock.Object, hashGenerator.Object, _specifications.Object, _mapper);

        //Act
        //Assert
        Assert.ThrowsAsync<ApiException>(async () => await pasteService.RemovePaste("lalala", Guid.Empty));
    }
}
