using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagment.Controllers
{
	public class ReportsController : Controller
	{
		public IActionResult Reports()
		{
			return View();
		}
	}
}
