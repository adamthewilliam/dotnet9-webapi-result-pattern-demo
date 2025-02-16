using Dotnet9.WebApi.ResultPattern.Demo.Contracts.Requests;
using Dotnet9.WebApi.ResultPattern.Demo.Data;
using Dotnet9.WebApi.ResultPattern.Demo.Data.Models;
using Dotnet9.WebApi.ResultPattern.Demo.Domain;
using Dotnet9.WebApi.ResultPattern.Demo.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Dotnet9.WebApi.ResultPattern.Demo.UnitTests;

public class PenguinServiceTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPenguinService _sut;

    public PenguinServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _dbContext = new ApplicationDbContext(options);
        _sut = new PenguinService(_dbContext);
    }

    [Fact]
    public async Task GetPenguinById_WhenPenguinDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var penguinId = Guid.NewGuid();

        // Act
        var result = await _sut.GetPenguinByIdAsync(penguinId);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Should().BeOfType<NotFoundError>()
            .Which.Message.Should().Contain(penguinId.ToString());
    }

    [Fact]
    public async Task GetPenguinById_WhenPenguinExists_ReturnsSuccessResult()
    {
        // Arrange
        var penguin = new PenguinModel
        {
            Id = Guid.NewGuid(),
            Name = "Happy",
            Species = "Emperor",
            Age = 5
        };
        
        await _dbContext.Penguins.AddAsync(penguin);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetPenguinByIdAsync(penguin.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(penguin.Id);
        result.Value.Name.Should().Be(penguin.Name);
    }

    [Fact]
    public async Task CreatePenguin_WhenSuccessful_ReturnsPenguin()
    {
        // Arrange
        var request = new CreatePenguinRequestDto(
            "Happy", 
            "Emperor", 
            5);

        // Act
        var result = await _sut.CreatePenguinAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Species.Should().Be(request.Species);
        result.Value.Age.Should().Be(request.Age);
        
        var savedPenguin = await _dbContext.Penguins.FindAsync(result.Value.Id);
        savedPenguin.Should().NotBeNull();
        savedPenguin!.Name.Should().Be(request.Name);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}