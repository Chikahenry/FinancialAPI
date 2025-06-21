using FinancialTransactionsAPI.Dtos;
using FinancialTransactionsAPI.Models;
using FinancialTransactionsAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancialTransactionsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction(CreateTransactionDto dto)
        {
            var userId = GetCurrentUserId();
            var transaction = await _transactionService.CreateTransactionAsync(dto, userId);
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<TransactionDto>>> GetTransactions([FromQuery] TransactionFilterDto filter)
        {
            var userId = GetCurrentUserId();
            var transactions = new PagedResult<TransactionDto>();
            if (IsAdmin())
                transactions = await _transactionService.GetTransactionsAsync(filter, null);
            else
                transactions = await _transactionService.GetTransactionsAsync(filter, userId);

            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            var userId = GetCurrentUserId();
            var transaction = new TransactionDto();
            if(IsAdmin())
                transaction= await _transactionService.GetTransactionByIdAsync(id, null);
            else
                transaction = await _transactionService.GetTransactionByIdAsync(id, userId);

            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        private bool IsAdmin()
        {
            return User.IsInRole("admin");
        }
    }
}
