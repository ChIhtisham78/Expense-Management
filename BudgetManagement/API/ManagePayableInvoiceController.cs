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
					AccountName = db.AccountEntities.FirstOrDefault(acc => acc.Id == invoice.AccountId) != null ? db.AccountEntities.FirstOrDefault(acc => acc.Id == invoice.AccountId).AccName : null,
					InvoiceAmount = invoice.Amount,
					InvoiceDate = invoice.InvoiceDate,
					TransactionAmount = db.Transactions
								.Where(t => t.InvoiceId == invoice.Id)
								.Sum(t => t.Amount),
					Balance = invoice.Amount - db.Transactions
						.Where(t => t.InvoiceId == invoice.Id)
						.Sum(t => t.Amount),
					InvoiceReffId = invoice.InvoiceReffId // Include InvoiceReffId in the result
				})
				.ToListAsync();
			return Ok(payableInvoices);
		}
	}
}
