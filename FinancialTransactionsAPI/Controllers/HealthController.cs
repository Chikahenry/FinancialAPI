using FinancialTransactionsAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTransactionsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HealthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                await _context.Database.CanConnectAsync();
                return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
            }
            catch
            {
                return StatusCode(500, new { Status = "Unhealthy", Timestamp = DateTime.UtcNow });
            }
        }
    }
}
