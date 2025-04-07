using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagment.Controllers
{
    public class SalaryMappingController : Controller
    { 
        public IActionResult Salary()
        {
            return View();
        }
    }
}
