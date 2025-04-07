using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagment.Controllers
{
	public class AuditLogController : Controller
	{
		public IActionResult AuditLog()
		{
			return View();
		}
	}
}
