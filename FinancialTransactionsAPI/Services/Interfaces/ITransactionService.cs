using FinancialTransactionsAPI.Dtos;

namespace FinancialTransactionsAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto, int userId);
        Task<PagedResult<TransactionDto>> GetTransactionsAsync(TransactionFilterDto filter, int? userId = null);
        Task<TransactionDto?> GetTransactionByIdAsync(int id, int? userId = null);
    }
}
