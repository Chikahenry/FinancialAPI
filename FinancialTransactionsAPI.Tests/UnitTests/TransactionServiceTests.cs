using FinancialTransactionsAPI.Data;
using FinancialTransactionsAPI.Dtos;
using FinancialTransactionsAPI.Models;
using FinancialTransactionsAPI.Services.Implementation;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FinancialTransactionsAPI.Tests.UnitTests
{
    public class TransactionServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new TransactionService(_context);

            SeedTestData();
        }

        [Fact]
        public async Task CreateTransactionAsync_ShouldCreateTransaction_WhenValidData()
        {
            // Arrange
            var dto = new CreateTransactionDto
            {
                Amount = 100.50m,
                Description = "Test transaction",
                Type = TransactionType.Expense,
                Category = "Test"
            };
            var userId = 1;

            // Act
            var result = await _service.CreateTransactionAsync(dto, userId);

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(dto.Amount);
            result.Description.Should().Be(dto.Description);
            result.Type.Should().Be(dto.Type);
            result.Category.Should().Be(dto.Category);
            result.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task GetTransactionsAsync_ShouldReturnUserTransactions_WhenUserIdProvided()
        {
            // Arrange
            var filter = new TransactionFilterDto { Page = 1, PageSize = 10 };
            var userId = 1;

            // Act
            var result = await _service.GetTransactionsAsync(filter, userId);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().OnlyContain(t => t.UserId == userId);
            result.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetTransactionsAsync_ShouldReturnAllTransactions_WhenNoUserIdProvided()
        {
            // Arrange
            var filter = new TransactionFilterDto { Page = 1, PageSize = 10 };

            // Act
            var result = await _service.GetTransactionsAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCountGreaterThan(1);
            result.Items.Should().Contain(t => t.UserId == 1);
            result.Items.Should().Contain(t => t.UserId == 2);
        }

        [Fact]
        public async Task GetTransactionsAsync_ShouldApplyFilters_WhenFiltersProvided()
        {
            // Arrange
            var filter = new TransactionFilterDto
            {
                Type = TransactionType.Income,
                MinAmount = 1000m,
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await _service.GetTransactionsAsync(filter);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().OnlyContain(t => t.Type == TransactionType.Income && t.Amount >= 1000m);
        }

        private void SeedTestData()
        {
            var users = new[]
            {
            new User { Id = 1, Username = "user1", Email = "user1@test.com", PasswordHash = "hash1", Role = "user" },
            new User { Id = 2, Username = "user2", Email = "user2@test.com", PasswordHash = "hash2", Role = "user" }
        };

            var transactions = new[]
            {
            new Transaction { Id = 1, Amount = 1500m, Description = "Salary", Type = TransactionType.Income, Category = "Salary", UserId = 1 },
            new Transaction { Id = 2, Amount = 50m, Description = "Groceries", Type = TransactionType.Expense, Category = "Food", UserId = 1 },
            new Transaction { Id = 3, Amount = 2000m, Description = "Freelance", Type = TransactionType.Income, Category = "Work", UserId = 2 }
        };

            _context.Users.AddRange(users);
            _context.Transactions.AddRange(transactions);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
