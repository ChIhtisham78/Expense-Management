using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagment.Controllers
{
    public class GenerateSalaryController : Controller
    {
        public IActionResult GenerateSalary()
        {
            return View();
        }
    }
}
