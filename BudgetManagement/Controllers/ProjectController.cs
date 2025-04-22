using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagment.Controllers
{
    public class ProjectController : Controller
    {
       public IActionResult Project()
        {
            return View();
        }
    }
}
