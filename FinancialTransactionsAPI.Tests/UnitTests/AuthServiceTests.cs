using FinancialTransactionsAPI.Data;
using FinancialTransactionsAPI.Dtos;
using FinancialTransactionsAPI.Models;
using FinancialTransactionsAPI.Services.Implementation;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FinancialTransactionsAPI.Tests.UnitTests
{
    public class AuthServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JwtSettings:SecretKey"] = "YourSuperSecretKeyThatIsAtLeast256BitsLong!"
                })
                .Build();

            _authService = new AuthService(_context, configuration);

            SeedTestData();
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenValidCredentials()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "password123"
            };

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeEmpty();
            result.Username.Should().Be("testuser");
            result.Role.Should().Be("user");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenInvalidCredentials()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_WhenValidData()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "newuser@test.com",
                Password = "password123"
            };

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().NotBeEmpty();
            result.Username.Should().Be("newuser");
            result.Role.Should().Be("user");

            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == "newuser");
            userInDb.Should().NotBeNull();
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenUsernameExists()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser", 
                Email = "newemail@test.com",
                Password = "password123"
            };

            // Act & Assert
            await _authService.Invoking(s => s.RegisterAsync(registerDto))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Username or email already exists");
        }

        private void SeedTestData()
        {
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "user"
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
