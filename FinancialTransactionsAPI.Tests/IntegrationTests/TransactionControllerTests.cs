using FinancialTransactionsAPI.Dtos;
using FinancialTransactionsAPI.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace FinancialTransactionsAPI.Tests.IntegrationTests
{
    public class TransactionsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public TransactionsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        [Fact]
        public async Task CreateTransaction_ShouldReturn401_WhenNotAuthenticated()
        {
            // Arrange
            var transaction = new CreateTransactionDto
            {
                Amount = 100m,
                Description = "Test transaction",
                Type = TransactionType.Expense,
                Category = "Test"
            };

            var json = JsonSerializer.Serialize(transaction, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/transactions", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateTransaction_ShouldReturn201_WhenAuthenticated()
        {
            // Arrange
            var token = await GetAuthTokenAsync("john_doe", "user123");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var transaction = new CreateTransactionDto
            {
                Amount = 100m,
                Description = "Test transaction",
                Type = TransactionType.Expense,
                Category = "Test"
            };

            var json = JsonSerializer.Serialize(transaction, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/transactions", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdTransaction = JsonSerializer.Deserialize<TransactionDto>(responseContent, _jsonOptions);

            createdTransaction.Should().NotBeNull();
            createdTransaction!.Amount.Should().Be(100m);
            createdTransaction.Description.Should().Be("Test transaction");
        }

        [Fact]
        public async Task GetTransactions_ShouldReturnUserTransactions_WhenUserAuthenticated()
        {
            // Arrange
            var token = await GetAuthTokenAsync("john_doe", "user123");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/transactions");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<TransactionDto>>(responseContent, _jsonOptions);

            result.Should().NotBeNull();
            result!.Items.Should().NotBeEmpty();
            result.Items.Should().OnlyContain(t => t.Username == "john_doe");
        }

        [Fact]
        public async Task GetTransactions_ShouldReturnAllTransactions_WhenAdminAuthenticated()
        {
            // Arrange
            var token = await GetAuthTokenAsync("admin", "admin123");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/transactions");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<TransactionDto>>(responseContent, _jsonOptions);

            result.Should().NotBeNull();
            result!.Items.Should().NotBeEmpty();
            result.Items.Should().Contain(t => t.Username == "john_doe");
            result.Items.Should().Contain(t => t.Username == "jane_smith");
        }

        [Fact]
        public async Task GetTransactions_ShouldApplyFilters_WhenParametersProvided()
        {
            // Arrange
            var token = await GetAuthTokenAsync("admin", "admin123");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/transactions?type=1&minAmount=100");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<TransactionDto>>(responseContent, _jsonOptions);

            result.Should().NotBeNull();
            result!.Items.Should().OnlyContain(t => t.Type == TransactionType.Expense && t.Amount >= 100);
        }

        private async Task<string> GetAuthTokenAsync(string username, string password)
        {
            var loginDto = new LoginDto
            {
                Username = username,
                Password = password
            };

            var json = JsonSerializer.Serialize(loginDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/login", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, _jsonOptions);

            return authResponse!.Token;
        }
    }
}
