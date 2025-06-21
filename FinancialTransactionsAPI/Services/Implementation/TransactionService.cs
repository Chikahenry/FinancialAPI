using FinancialTransactionsAPI.Data;
using FinancialTransactionsAPI.Dtos;
using FinancialTransactionsAPI.Models;
using FinancialTransactionsAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancialTransactionsAPI.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto, int userId)
        {
            var transaction = new Transaction
            {
                Amount = dto.Amount,
                Description = dto.Description,
                Type = dto.Type,
                Category = dto.Category,
                TransactionDate = dto.TransactionDate ?? DateTime.UtcNow,
                UserId = userId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return await GetTransactionDtoAsync(transaction.Id);
        }

        public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(TransactionFilterDto filter, int? userId = null)
        {
            var query = _context.Transactions
                .Include(t => t.User)
                .AsQueryable();

            // Apply user filter for non-admin users
            if (userId.HasValue)
            {
                query = query.Where(t => t.UserId == userId.Value);
            }

            // Apply filters
            if (filter.FromDate.HasValue)
                query = query.Where(t => t.TransactionDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(t => t.TransactionDate <= filter.ToDate.Value);

            if (filter.Type.HasValue)
                query = query.Where(t => t.Type == filter.Type.Value);

            if (!string.IsNullOrEmpty(filter.Category))
                query = query.Where(t => t.Category.Contains(filter.Category));

            if (filter.MinAmount.HasValue)
                query = query.Where(t => t.Amount >= filter.MinAmount.Value);

            if (filter.MaxAmount.HasValue)
                query = query.Where(t => t.Amount <= filter.MaxAmount.Value);

            var totalCount = await query.CountAsync();

            var transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Description = t.Description,
                    Type = t.Type,
                    Category = t.Category,
                    TransactionDate = t.TransactionDate,
                    CreatedAt = t.CreatedAt,
                    UserId = t.UserId,
                    Username = t.User.Username
                })
                .ToListAsync();

            return new PagedResult<TransactionDto>
            {
                Items = transactions,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int id, int? userId = null)
        {
            var query = _context.Transactions
                .Include(t => t.User)
                .Where(t => t.Id == id);

            if (userId.HasValue)
            {
                query = query.Where(t => t.UserId == userId.Value);
            }

            var transaction = await query.FirstOrDefaultAsync();

            if (transaction == null) return null;

            return new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Type = transaction.Type,
                Category = transaction.Category,
                TransactionDate = transaction.TransactionDate,
                CreatedAt = transaction.CreatedAt,
                UserId = transaction.UserId,
                Username = transaction.User.Username
            };
        }

        private async Task<TransactionDto> GetTransactionDtoAsync(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.User)
                .FirstAsync(t => t.Id == id);

            return new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Type = transaction.Type,
                Category = transaction.Category,
                TransactionDate = transaction.TransactionDate,
                CreatedAt = transaction.CreatedAt,
                UserId = transaction.UserId,
                Username = transaction.User.Username
            };
        }
    }
}
