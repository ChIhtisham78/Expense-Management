using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagment.Controllers
{
    public class ManageExpenceController : Controller
    {
        public IActionResult AddExpence()
        {
            return View();
        }
    }
}
