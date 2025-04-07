using ExpenseManagment.Data;
using ExpenseManagment.Filters;
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
                var data = await db.AuditLogs
                    .Select(g => new
                    {
                        g.Timestamp,
                        g.UserEmail,
                        g.UserId,
                        g.ActionType,
                        g.TableName,
                        g.KeyValues,
                        g.OldValues,
                        g.NewValues,
                    })
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
