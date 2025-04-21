using System.Diagnostics;
using ExpenseManagment.Custom;
using ExpenseManagment.Data;
using ExpenseManagment.Data.Common;
using ExpenseManagment.Data.DataBaseEntities;
using ExpenseManagment.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagment.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageInvoiceController : ControllerBase
    {
        private readonly ApplicationDbContext db;
        public ManageInvoiceController(ApplicationDbContext _db)
        {
            db = _db;
        }

        [HttpGet("BusinessCapitalTransaction")]
        [Authorize(Roles = Helper.RolesAttrVal.AddCapitalToBusiness)]
        public async Task<IActionResult> GetBusinessCapitalTransaction()
        {
            var result = await db.Transactions
                .Include(x => x.Invoice)
                .Include(x => x.Invoice.AccountEntities)
                .Where(x => x.Invoice.InvoiceType == (int)Helper.InvoiceTypeId.BusinessCapitalAccountTransaction)
                .OrderByDescending(x => x.Id)
                .Select(x => new
                {
                    TransactionId = x.Id,
                    x.TransactionDate,
                    x.Amount,
                    Payee = x.Invoice.AccountEntities.AccName,
                    Recipient = db.AccountEntities.FirstOrDefault(c => c.Id == x.BeneficiaryAccountId).AccName,
                    x.Desc,
                }).ToListAsync();

            return Ok(result);
        }

        [AjaxExceptionFilter]
        [HttpPost("BusinessCapitalTransaction")]
        public async Task<IActionResult> PostBusinessCapitalTransaction(Transaction model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid model state");

            try
            {
                var capitalAccount = await db.AccountEntities
                    .SingleOrDefaultAsync(x => x.AccountTypeId == (int)Helper.AccountTypeId.BusinessCapitalAccount);

                if (capitalAccount == null)
                    return NotFound("Business Capital Account not found");

                var invoice = new Invoice
                {
                    AccountId = capitalAccount.Id,
                    IsPayable = false,
                    Amount = model.Amount,
                    Desc = model.Desc,
                    InvoiceReffId = 0,
                    InvoiceDate = model.TransactionDate,
                    InvoiceType = (int)Helper.InvoiceTypeId.BusinessCapitalAccountTransaction
                };

                await db.Invoices.AddAsync(invoice);
                await db.SaveChangesAsync();

                var transaction = new Transaction
                {
                    InvoiceId = invoice.Id,
                    Amount = model.Amount,
                    BeneficiaryAccountId = model.BeneficiaryAccountId,
                    Desc = model.Desc,
                    TransactionDate = model.TransactionDate,
                    CreateOn = DateTime.Now,
                    CreatedBy = User.Identity?.Name
                };

                await db.Transactions.AddAsync(transaction);

                await db.SaveChangesAsync();
                return Ok("Business capital transaction saved successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("ProjectTransaction")]
        [Authorize(Roles = Helper.RolesAttrVal.ProjectTransaction)]
        public async Task<IActionResult> ProjectTransaction()
        {
            try
            {
                var projectTransactions = await (
                    from invoice in db.Invoices
                    join trans in db.Transactions on invoice.Id equals trans.InvoiceId into transGroup
                    from trans in transGroup.DefaultIfEmpty()
                    join account in db.AccountEntities on invoice.AccountId equals account.Id into accountGroup
                    from account in accountGroup.DefaultIfEmpty()
                    join project in db.Projects on invoice.InvoiceReffId equals project.Id into projectGroup
                    from project in projectGroup.DefaultIfEmpty()
                    where invoice.InvoiceType == (int)Helper.InvoiceTypeId.BusinessInvoice
                    select new
                    {
                        InvoiceId = invoice.Id,
                        ProjectId = project.Id,
                        ProjectName = project.ProjectName,
                        AccountId = account.Id,
                        AccountName = account.AccName,
                        InvoiceAmount = invoice.Amount,
                        InvoiceReffId = invoice.InvoiceReffId,
                        TransactionDate = trans.TransactionDate,
                        Description = invoice.Desc
                    }
                ).ToListAsync();

                return Ok(projectTransactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [AjaxExceptionFilter]
        [HttpPost("ProjectTransaction")]
        public async Task<IActionResult> ProjectTransaction(ProjectTransactionModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(400, Helper.InvalidModelState);
                }


                var invoice = new Invoice
                {
                    AccountId = model.ClientId,
                    IsPayable = false,
                    Amount = model.Amount,
                    Desc = model.Desc,
                    InvoiceReffId = model.ProjectId,
                    InvoiceDate = model.TransactionDate,
                    InvoiceType = (int)Helper.InvoiceTypeId.BusinessInvoice
                };

                await db.DbSaveChangesAsync();

                var transaction = new Transaction
                {
                    InvoiceId = invoice.Id,
                    Amount = model.Amount,
                    BeneficiaryAccountId = (int)Helper.AccountTypeId.IncomeAccountOfBusiness,
                    Desc = model.Desc,
                    TransactionDate = model.TransactionDate,
                    CreateOn = DateTime.Now,
                    CreatedBy = User.Identity?.Name
                };

                await db.Transactions.AddAsync(transaction);
                await db.SaveChangesAsync();

                return Ok("Project transaction saved successfully");

            }
            catch (Exception exp)
            {
                return StatusCode(500, Helper.ObjectNotFound + exp.Message);
            }
        }
    }
}