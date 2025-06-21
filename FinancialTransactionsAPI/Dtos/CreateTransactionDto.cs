using FinancialTransactionsAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace FinancialTransactionsAPI.Dtos
{
    public class CreateTransactionDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        public DateTime? TransactionDate { get; set; }
    }
}
