using ExpenseManagment.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagment.API
{
    public class BaseController : Controller
    {
        public JsonResult ResponseResult<T>(T data, string message = null)
        {
            return Json(
                new ResponseData<T>
                {
                    IsValid = true,
                    Message = message == null ? "Successfully Processed" : message,
                    Data = data
                });
        }
    }
}
