using ExpenseManagment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace ExpenseManagment.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AjaxExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Log.Error(context.Exception, "Error occurred");
            context.Result = new OkObjectResult(
                new ResponseErrorData
                {
                    IsValid = false,
                    Message = context.Exception.Message
                });

            context.ExceptionHandled = true;
        }
    }
}
