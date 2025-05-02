using ExpenseManagment.Custom;
using ExpenseManagment.Data;
using ExpenseManagment.Data.DataBaseEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseManagment.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageReportsController : Controller
    {
        private readonly ApplicationDbContext db;
        public ManageReportsController(ApplicationDbContext _db)
        {
            db = _db;
        }

        [HttpGet("GetReport")]
        public async Task<IActionResult> GetReport(int? accountId)
        {
            var query = db.Transactions
                .Include(x => x.Invoice)
                    .ThenInclude(x => x.AccountEntities)
                .Where(x => x.Invoice.InvoiceType == (int)Helper.InvoiceTypeId.BusinessCapitalAccountTransaction);

            if (accountId.HasValue && accountId.Value != 0)
            {
                query = query.Where(x =>
                    x.Invoice.AccountId == accountId.Value ||
                    x.BeneficiaryAccountId == accountId.Value);
            }

            var data = await query
                .OrderByDescending(x => x.Id)
                .Select(x => new
                {
                    TransactionId = x.Id,
                    x.TransactionDate,
                    x.Amount,
                    Payee = x.Invoice.AccountEntities.AccName,
                    Recipient = db.AccountEntities
                        .Where(c => c.Id == x.BeneficiaryAccountId)
                        .Select(c => c.AccName)
                        .FirstOrDefault(),
                    x.Desc
                })
                .ToListAsync();

            return Ok(data);
        }


        // For Outward Reports
        [HttpGet("ReportGenerator")]
        public IActionResult ReportGenerator(int? accountId)
        {
            var query = db.Expences
                .Include(x => x.Account).AsQueryable();

            if (accountId.HasValue && accountId != 0)
            {
                query = query.Where(x => x.AccountId == accountId);
            }

            var data = query.Select(e => new
            {
                Id = e.Id,
                Amount = e.ExpenceAmount,
                Date = e.ExpenceDate,
                Description = e.ExpenceDesc,
                Recipient = db.AccountEntities.FirstOrDefault(c => c.AccName == Helper.DefaultExpenceAccount).AccName,
                AccountName = e.Account.AccName
            }).ToList();

            return Ok(data);
        }

        [HttpGet("GetCurrentUserInfo")]
        public IActionResult GetCurrentUserInfo()
        {
            // Retrieve the user's identity from the current HttpContext
            var userIdentity = HttpContext.User.Identity as ClaimsIdentity;

            // Assuming you have a claim with the user's name
            var userNameClaim = userIdentity.FindFirst(ClaimTypes.Name);
            if (userNameClaim != null)
            {
                var userName = userNameClaim.Value;
                return Ok(new { UserName = userName });
            }
            else
            {
                return NotFound(); // User's name not found in claims
            }
        }



        [HttpGet("GetInitialBalance")]
        public async Task<IActionResult> GetInitialBalance(int? accountId)
        {
            IQueryable<Transaction> inwardQuery = db.Transactions
                .Where(x => x.Invoice.InvoiceType == (int)Helper.InvoiceTypeId.BusinessCapitalAccountTransaction);

            IQueryable<Expence> outwardQuery = db.Expences.AsQueryable();

            if (accountId.HasValue && accountId != 0)
            {
                inwardQuery = inwardQuery.Where(x => x.Invoice.AccountId == accountId || x.BeneficiaryAccountId == accountId);
                outwardQuery = outwardQuery.Where(x => x.AccountId == accountId);
            }

            var inwardTotal = await inwardQuery.SumAsync(x => x.Amount);
            var outwardTotal = await outwardQuery.SumAsync(x => x.ExpenceAmount);

            var balance = inwardTotal - outwardTotal;

            return Ok(balance);
        }
    }
}
