using ChatApp.Business.Services;
using ChatApp.Data.Entities;
using ChatApp.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatApp.Business.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly TokenManager _tokenManager;
    private readonly AuthService _sut; // SUT = System Under Test

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        // Mock configuration
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:Key"]).Returns("test_secret_key_123456789");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _tokenManager = new TokenManager(configMock.Object);

        _sut = new AuthService(_userRepoMock.Object, _tokenManager, _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFailure_WhenUsernameAlreadyExists()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>()))
                     .ReturnsAsync(true);

        // Act
        var result = await _sut.RegisterAsync("adam", "password123");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Username already exists", result.Message);
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenUserIsCreated()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>()))
                     .ReturnsAsync(false);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<UserEntity>())).ReturnsAsync(true);

        // Act
        var result = await _sut.RegisterAsync("newuser", "password123");

        // Assert
        Assert.True(result.Success);
        Assert.Equal("User created successfully", result.Message);
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorized_WhenUserNotFound()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>())).ReturnsAsync((UserEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync("unknownuser", "password123"));
        _userRepoMock.Verify(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserEntity, bool>>>()), Times.Once);
    }
}
