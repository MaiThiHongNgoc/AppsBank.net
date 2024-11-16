using Microsoft.AspNetCore.Mvc;
using ATMBank.Models;
using Microsoft.EntityFrameworkCore;
using ATMBank.Data;

namespace ATMBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ATMContext _context;

        public AccountController(ATMContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts.Include(a => a.User).Include(a => a.Transactions).ToListAsync();
        }

        // ApI get account by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var account = await _context.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.AccountId == id);

            if (account == null) return NotFound("Account not found");
            return Ok(account);
        }

        // ApI create new account
        [HttpPost]
        public async Task<IActionResult> CreateAccount(Account account)
        {
            _context.Accounts.Add(account); //save account to Dbset
            await _context.SaveChangesAsync();//insert to db
            return CreatedAtAction(nameof(GetAccount), new { id = account.AccountId }, account);
        }

        // API Deposit money
        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> Deposit(int id, [FromBody] Transaction request)
        {
            if (request == null || request.Amount <= 0)
                return BadRequest("Invalid amount.");

            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound("Account not found");

            account.Balance += request.Amount;

            var transaction = new Transaction
            {
                AccountId = id,
                Amount = request.Amount,
                Isuccessful = true,
                Description = "Deposit"
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { account.Balance });
        }

        // API Withdraw money
        [HttpPost("{id}/withdraw")]
        public async Task<IActionResult> Withdraw(int id, [FromBody] Transaction request)
        {
            // Kiểm tra tài khoản có tồn tại hay không
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound("Account not found");

            // Kiểm tra số tiền có hợp lệ không
            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than 0.");

            // Kiểm tra số dư có đủ không
            if (account.Balance < request.Amount)
                return BadRequest("Insufficient balance.");

            // Trừ số dư tài khoản
            account.Balance -= request.Amount;

            // Tạo giao dịch mới
            var transaction = new Transaction
            {
                AccountId = id,
                Amount = -request.Amount, // Lưu số tiền rút (âm)
                Isuccessful = true,
                Description = request.Description ?? "Withdrawal", // Nếu không có mô tả, mặc định là "Withdrawal"
                Timestemp = DateTime.Now
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Trả về số dư mới
            return Ok(new { account.Balance });
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, Account account)
        {
            if (id != account.AccountId)
                return BadRequest();

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Accounts.Any(e => e.AccountId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return NotFound();

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
