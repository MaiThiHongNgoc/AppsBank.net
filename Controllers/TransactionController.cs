using Microsoft.AspNetCore.Mvc;
using ATMBank.Models;
using Microsoft.EntityFrameworkCore;
using ATMBank.Data;

namespace ATMBank.Controllers{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase{
        private readonly ATMContext _context;

        public TransactionController(ATMContext context){
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(){
            return await _context.Transactions.Include(t => t.Account).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id){
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == id);

            if (transaction == null)
                return NotFound();

            return transaction;
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction){
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.TransactionId }, transaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction transaction){
            if (id != transaction.TransactionId)
                return BadRequest();

            _context.Entry(transaction).State = EntityState.Modified;

            try{
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException){
                if (!_context.Transactions.Any(e => e.TransactionId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id){
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
