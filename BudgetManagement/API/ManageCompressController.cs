using ExpenseManagment.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagment.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class ManageCompressController : Controller
	{
		private readonly ApplicationDbContext db;
		public ManageCompressController(ApplicationDbContext _db)
		{
			db = _db;
		}


		[HttpGet("GetCompressTable")]
		public async Task<IActionResult> GetCompressTable()
		{
			try
			{
				var data = await db.Expences
					.Select(g => new
					{
						AccountName = g.Account.AccName,
						Date = g.ExpenceDate,
						ExpenceType = g.ExpenceType,
						Amount = g.ExpenceAmount,
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
