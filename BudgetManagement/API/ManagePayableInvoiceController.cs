using ExpenseManagment.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagment.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class ManagePayableInvoiceController : Controller
	{
		private readonly ApplicationDbContext db;

		public ManagePayableInvoiceController(ApplicationDbContext _db)
		{
			db = _db;
		}

        [HttpGet("GetPayableInvoices")]
        public async Task<IActionResult> GetPayableInvoices()
        {
            var payableInvoices = await db.Invoices
                .Where(invoice => invoice.IsPayable)
                .Select(invoice => new
                {
                    AccountName = db.AccountEntities
                        .Where(acc => acc.Id == invoice.AccountId)
                        .Select(acc => acc.AccName)
                        .FirstOrDefault(),

                    InvoiceAmount = invoice.Amount,
                    InvoiceDate = invoice.InvoiceDate,
                    InvoiceReffId = invoice.InvoiceReffId,

                    TransactionAmount = db.Transactions
                        .Where(t => t.InvoiceId == invoice.Id)
                        .Sum(t => (decimal?)t.Amount) ?? 0,

                    Balance = invoice.Amount - (db.Transactions
                        .Where(t => t.InvoiceId == invoice.Id)
                        .Sum(t => (decimal?)t.Amount) ?? 0)
                })
                .ToListAsync();

            return Ok(payableInvoices);
        }

    }
}
