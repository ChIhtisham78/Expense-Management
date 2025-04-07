using ExpenseManagment.Custom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagment.Controllers
{
    public class AccountController : Controller
    {
        [Authorize(Roles = Helper.RolesAttrVal.Client)]
        public IActionResult Client()
        {
            return View();
        }

        [Authorize(Roles = Helper.RolesAttrVal.Business)]
        public IActionResult Business()
        {
            return View();
        }

        [Authorize(Roles = Helper.RolesAttrVal.Partner)]
        public IActionResult Partner()
        {
            return View();
        }

        [Authorize(Roles = Helper.RolesAttrVal.Contractor)]
        public IActionResult Contractor()
        {
            return View();
        }

        [Authorize(Roles = Helper.RolesAttrVal.Project)]
        public IActionResult Project()
        {
            return View();
        }
    }
}
