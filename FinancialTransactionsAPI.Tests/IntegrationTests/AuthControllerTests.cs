using FinancialTransactionsAPI.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace FinancialTransactionsAPI.Tests.IntegrationTests
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenValidCredentials()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "admin",
                Password = "admin123"
            };

            var json = JsonSerializer.Serialize(loginDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, _jsonOptions);

            authResponse.Should().NotBeNull();
            authResponse!.Token.Should().NotBeEmpty();
            authResponse.Username.Should().Be("admin");
            authResponse.Role.Should().Be("admin");
        }

        [Fact]
        public async Task Login_ShouldReturn401_WhenInvalidCredentials()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "admin",
                Password = "wrongpassword"
            };

            var json = JsonSerializer.Serialize(loginDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Register_ShouldReturnToken_WhenValidData()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser123",
                Email = "newuser123@test.com",
                Password = "password123"
            };

            var json = JsonSerializer.Serialize(registerDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, _jsonOptions);

            authResponse.Should().NotBeNull();
            authResponse!.Token.Should().NotBeEmpty();
            authResponse.Username.Should().Be("newuser123");
            authResponse.Role.Should().Be("user");
        }

        [Fact]
        public async Task Register_ShouldReturn400_WhenUsernameExists()
        {
            var registerDto = new RegisterDto
            {
                Username = "admin", 
                Email = "newemail@test.com",
                Password = "password123"
            };

            var json = JsonSerializer.Serialize(registerDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
