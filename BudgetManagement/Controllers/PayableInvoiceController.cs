using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagment.Controllers
{
	public class PayableInvoiceController : Controller
	{
		public IActionResult PayInvoice()
		{
			return View();
		}
	}
}
