using ExpenseManagment.Data;
using ExpenseManagment.Data.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
