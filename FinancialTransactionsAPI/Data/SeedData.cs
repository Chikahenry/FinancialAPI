using FinancialTransactionsAPI.Models;

namespace FinancialTransactionsAPI.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any()) return;

            var users = new[]
            {
            new User
            {
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "admin"
            },
            new User
            {
                Username = "john_doe",
                Email = "john@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                Role = "user"
            },
            new User
            {
                Username = "jane_smith",
                Email = "jane@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                Role = "user"
            }
        };

            context.Users.AddRange(users);
            context.SaveChanges();

            var transactions = new[]
            {
            new Transaction
            {
                Amount = 1500.00m,
                Description = "Salary payment",
                Type = TransactionType.Income,
                Category = "Salary",
                UserId = users[1].Id,
                TransactionDate = DateTime.UtcNow.AddDays(-10)
            },
            new Transaction
            {
                Amount = 50.00m,
                Description = "Grocery shopping",
                Type = TransactionType.Expense,
                Category = "Food",
                UserId = users[1].Id,
                TransactionDate = DateTime.UtcNow.AddDays(-5)
            },
            new Transaction
            {
                Amount = 2000.00m,
                Description = "Freelance project",
                Type = TransactionType.Income,
                Category = "Freelance",
                UserId = users[2].Id,
                TransactionDate = DateTime.UtcNow.AddDays(-7)
            }
        };

            context.Transactions.AddRange(transactions);
            context.SaveChanges();
        }
    }
}
