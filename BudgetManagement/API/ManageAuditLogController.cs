using System.IO;
using ExpenseManagment.Data;
using ExpenseManagment.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagment.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageAuditLogController : Controller
    {
        private readonly ApplicationDbContext db;

        public ManageAuditLogController(ApplicationDbContext _db)
        {
            db = _db;
        }


        [AjaxExceptionFilter]
        [HttpGet("GetAuditLog")]
        public async Task<IActionResult> GetAuditLog()
        {
            try
            {
                var auditLog = db.AuditLogs.ToListAsync();
                return Ok(auditLog);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

    }
}