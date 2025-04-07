using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagment.Controllers
{
	public class UserManagementController : Controller
	{
		public IActionResult Management()
		{
			return View();
		}
	}
}
