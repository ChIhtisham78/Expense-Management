using ExpenseManagment.Custom;
using ExpenseManagment.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagment.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetddlController : ControllerBase
    {
        private readonly ApplicationDbContext db;
        public GetddlController(ApplicationDbContext _db)
        {
            db = _db;
        }
        [HttpGet("ExpenseAccounts")]
        public async Task<IActionResult> ExpenseAccounts()
        {
            var expenseAccountIds = await db.Expences
                .Where(x => x.AccountId != null)
                .Select(x => x.AccountId)
                .Distinct()
                .ToListAsync();

            var transactionAccountIds = await db.Transactions
                .Where(x => x.BeneficiaryAccountId != null && x.BeneficiaryAccountId != 7)
                .Select(x => x.BeneficiaryAccountId)
                .Distinct()
                .ToListAsync();
            var uniqueAccountIds = expenseAccountIds.Union(transactionAccountIds).ToList();

            var expenseAccounts = await db.AccountEntities
                .Where(a => uniqueAccountIds.Contains(a.Id))
                .Select(a => new
                {
                    AccountId = a.Id,
                    AccountName = a.AccName
                })
                .ToListAsync();

            return Ok(expenseAccounts);
        }



        [HttpGet("Clients")]
        public async Task<IActionResult> Clients()
        {
            return Ok(await db.AccountEntities.Where(x => x.AccountTypeId == (int)Helper.AccountTypeId.client).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.AccName.ToString()
            }).ToListAsync());
        }


        [HttpGet("BusinessAcc")]
        public async Task<IActionResult> BusinessAcc()
        {
            return Ok(await db.AccountEntities.Where(x => x.AccountTypeId == (int)Helper.AccountTypeId.business).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.AccName.ToString()
            }).ToListAsync());
        }


        [HttpGet("GetProjectWRTType/{id}")]
        public async Task<IActionResult> GetProjectWRTType(int id)
        {
            return Ok(await db.Projects.Where(x => x.ClientId == id).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.ProjectName.ToString()
            }).ToListAsync());
        }

        [HttpGet("Contractor")]
        public async Task<IActionResult> Contractor()
        {
            return Ok(await db.AccountEntities.Where(x => x.AccountTypeId == (int)Helper.AccountTypeId.contractor).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.AccName.ToString()
            }).ToListAsync());
        }

        [HttpGet("Project")]
        public async Task<IActionResult> Project()
        {
            return Ok(await db.Projects.Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.ProjectName.ToString()
            }).ToListAsync());
        }

        [HttpGet("SingleProject")]
        public async Task<IActionResult> SingleProject()
        {
            return Ok(await db.Projects.Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.ProjectName.ToString()
            }).ToListAsync());
        }
    }
}
