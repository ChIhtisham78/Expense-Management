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
            return Ok(await db.Transactions.
                Include(x => x.Invoice).
                Include(x => x.Invoice.AccountEntities).
                Where(x => x.Invoice.InvoiceType == (int)Helper.InvoiceTypeId.BusinessCapitalAccountTransaction).
                OrderByDescending(x => x.Id).
                Select(x => new
                {
                    TransactionId = x.Id,
                    x.TransactionDate,
                    x.Amount,
                    Payee = x.Invoice.AccountEntities.AccName,
                    Recipient = db.AccountEntities.FirstOrDefault(c => c.Id == x.BeneficiaryAccountId).AccName,
                    x.Desc,
                }).ToListAsync());
        }
        [AjaxExceptionFilter]
        [HttpPost("BusinessCapitalTransaction")]
        public async Task<IActionResult> PostBusinessCapitalTransaction(Transaction model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int CapitalAccountInDbId = db.AccountEntities.Single(x => x.AccountTypeId == (int)Helper.AccountTypeId.BusinessCapitalAccount).Id;

                    Invoice invoice = new Invoice();
                    invoice.AccountId = CapitalAccountInDbId;
                    invoice.IsPayable = false;
                    invoice.Amount = model.Amount;
                    invoice.Desc = model.Desc;
                    invoice.InvoiceReffId = 0;// Capital Transaction Reff will be 0
                    invoice.InvoiceDate = model.TransactionDate;
                    invoice.InvoiceType = (int)Helper.InvoiceTypeId.BusinessCapitalAccountTransaction;
                    db.Invoices.Add(invoice);
                    await db.DbSaveChangesAsync();

                    Transaction transaction = new Transaction();
                    transaction.InvoiceId = invoice.Id;
                    transaction.Amount = model.Amount;
                    transaction.BeneficiaryAccountId = model.BeneficiaryAccountId;
                    transaction.Desc = model.Desc;
                    transaction.TransactionDate = model.TransactionDate;
                    transaction.CreateOn = DateTime.Now;
                    transaction.CreatedBy = User.Identity.Name;
                    db.Transactions.Add(transaction);
                    if (await db.DbSaveChangesAsync())
                    {
                        return Ok();
                    }
                    return StatusCode(500, Helper.ErrorInSaveChanges);
                }
                return StatusCode(500, Helper.InvalidModelState);
            }
            catch (Exception exp)
            {
                return StatusCode(500, Helper.ObjectNotFound + exp.Message);
            }
        }

        [HttpGet("ProjectTransaction")]
        [Authorize(Roles = Helper.RolesAttrVal.ProjectTransaction)]

        public async Task<IActionResult> ProjectTransaction()
        {
            var result = await (from invoice in db.Invoices
                                join trans in db.Transactions
                                on invoice.Id equals trans.InvoiceId
                                join accname in db.AccountEntities
                                on invoice.AccountId equals accname.Id
                                join proj in db.Projects
                                on invoice.InvoiceReffId equals proj.Id
                                where invoice.InvoiceType == (int)Helper.InvoiceTypeId.BusinessInvoice
                                select new
                                {
                                    Id = invoice.Id,
                                    ProjectId = proj.Id,
                                    AccountId = accname.Id,
                                    accname.AccName,
                                    proj.ProjectName,
                                    invoice.Amount,
                                    invoice.InvoiceReffId,
                                    trans.TransactionDate,
                                    invoice.Desc,

                                }).ToListAsync();
            return Ok(result);
        }

        [AjaxExceptionFilter]
        [HttpPost("ProjectTransaction")]
        public async Task<IActionResult> ProjectTransaction(ProjectTransactionModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    Invoice invoice = new Invoice();
                    invoice.AccountId = model.ClientId;
                    invoice.IsPayable = true;
                    invoice.Amount = model.Amount;
                    invoice.Desc = model.Desc;
                    invoice.InvoiceReffId = model.ProjectId;
                    invoice.InvoiceDate = model.TransactionDate;
                    invoice.InvoiceType = (int)Helper.InvoiceTypeId.BusinessInvoice;
                    db.Invoices.Add(invoice);
                    await db.DbSaveChangesAsync();

                    Transaction transaction = new Transaction();
                    transaction.InvoiceId = invoice.Id;
                    transaction.Amount = model.Amount;
                    transaction.BeneficiaryAccountId = (int)Helper.AccountTypeId.IncomeAccountOfBusiness;
                    transaction.Desc = model.Desc;
                    transaction.TransactionDate = model.TransactionDate;
                    transaction.CreateOn = DateTime.Now;
                    transaction.CreatedBy = User.Identity.Name;
                    db.Transactions.Add(transaction);
                    if (await db.DbSaveChangesAsync())
                    {
                        return Ok();
                    }
                    return StatusCode(500, Helper.ErrorInSaveChanges);
                }
                return StatusCode(500, Helper.InvalidModelState);
            }
            catch (Exception exp)
            {
                return StatusCode(500, Helper.ObjectNotFound + exp.Message);
            }
        }
    }
}