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
    public class ManageExpenceController : ControllerBase
    {
        private readonly ApplicationDbContext db;
        public ManageExpenceController(ApplicationDbContext _db)
        {
            db = _db;
        }

        [HttpGet("Expences")]
        [Authorize(Roles = Helper.RolesAttrVal.Expence)]
        public async Task<IActionResult> Expences()
        {
            var expense = await db.Expences
                .Include(x => x.Account)
                .ToListAsync();

            return Ok(expense);

        }


        //[HttpGet("DeletedExpences")]
        //public async Task<IActionResult> GetDeletedExpences()
        //{
        //    return Ok(await db.DeletedExpences.Include(x => x.Account).ToListAsync());
        //}

        [HttpGet("Expence/{id}")]
        public async Task<IActionResult> GetExpences(int id)
        {
            var expense = await db.Expences.Include(x => x.Account).FirstOrDefaultAsync(x => x.Id == id);
            return Ok(expense);
        }
        [AjaxExceptionFilter]
        [HttpPost("Expence")]

        public async Task<IActionResult> PostExpence(ExpenceModel model)
        {


            try
            {
                if (ModelState.IsValid)
                {
                    int DefaultExpenceAccountId = db.AccountEntities.Single(x => x.AccountTypeId == (int)Helper.AccountTypeId.GeneralExpenceAccountOfBusiness).Id;
                    var newExpence = new Expence
                    {
                        ExpenceType = model.ExpenceType,
                        ExpenceDate = model.ExpenceDate,
                        ExpenceDesc = model.ExpenceDesc,
                        AccountId = model.AccountId,
                        ExpenceAmount = model.ExpenceAmount,
                        InsertionDate = DateTime.Now
                    };
                    db.Expences.Add(newExpence);
                    await db.DbSaveChangesAsync();
                    //await db.SaveChangesAsync();

                    Invoice invoice = new Invoice();
                    invoice.AccountId = DefaultExpenceAccountId;
                    invoice.IsPayable = true;
                    invoice.Amount = newExpence.ExpenceAmount;
                    invoice.Desc = newExpence.ExpenceDesc;
                    invoice.InvoiceReffId = newExpence.Id;
                    invoice.InvoiceDate = newExpence.ExpenceDate;
                    invoice.InvoiceType = (int)Helper.InvoiceTypeId.ExpenceFromBusinessAccount;
                    db.Invoices.Add(invoice);
                    await db.DbSaveChangesAsync();
                    //await db.SaveChangesAsync();

                    Transaction transaction = new Transaction();
                    transaction.InvoiceId = invoice.Id;
                    transaction.Amount = newExpence.ExpenceAmount;
                    transaction.Desc = newExpence.ExpenceDesc;
                    transaction.TransactionDate = newExpence.ExpenceDate;
                    transaction.BeneficiaryAccountId = newExpence.AccountId;
                    transaction.CreateOn = DateTime.Now;
                    transaction.CreatedBy = User.Identity.Name;
                    db.Transactions.Add(transaction);
                    //await db.SaveChangesAsync();

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


        [AjaxExceptionFilter]
        [HttpPut("Expence")]
        public async Task<IActionResult> PutExpence(ExpenceModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data submitted.");

            try
            {
                var expence = await db.Expences.FirstOrDefaultAsync(x => x.Id == model.Id);
                var invoice = await db.Invoices.FirstOrDefaultAsync(x =>
                    x.InvoiceReffId == model.Id &&
                    x.InvoiceType == (int)Helper.InvoiceTypeId.ExpenceFromBusinessAccount);

                if (expence == null || invoice == null)
                    return NotFound("Expense or related invoice not found.");

                var transaction = await db.Transactions.FirstOrDefaultAsync(x => x.InvoiceId == invoice.Id);
                if (transaction == null)
                    return NotFound("Transaction not found.");

                expence.ExpenceType = model.ExpenceType;
                expence.ExpenceDate = model.ExpenceDate;
                expence.ExpenceDesc = model.ExpenceDesc;
                expence.ExpenceAmount = model.ExpenceAmount;
                expence.AccountId = model.AccountId;

                invoice.Amount = model.ExpenceAmount;
                invoice.Desc = model.ExpenceDesc;
                invoice.InvoiceDate = model.ExpenceDate;

                transaction.Amount = model.ExpenceAmount;
                transaction.Desc = model.ExpenceDesc;
                transaction.TransactionDate = model.ExpenceDate;
                transaction.BeneficiaryAccountId = model.AccountId;

                db.UpdateRange(expence, invoice, transaction);

                var result = await db.DbSaveChangesAsync();
                return result
                    ? Ok("Expense updated successfully.")
                    : StatusCode(500, Helper.ErrorInSaveChanges);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AjaxExceptionFilter]
        [HttpDelete("DeleteExpence/{id}")]
        public async Task<IActionResult> DeleteExpence(int id)
        {
            try
            {
                var expence = await db.Expences.FirstOrDefaultAsync(x => x.Id == id);
                if (expence == null)
                    return NotFound("Expense not found.");

                var invoice = await db.Invoices.FirstOrDefaultAsync(x =>
                    x.InvoiceReffId == id &&
                    x.InvoiceType == (int)Helper.InvoiceTypeId.ExpenceFromBusinessAccount);

                if (invoice == null)
                    return NotFound("Invoice not found for the expense.");

                var transaction = await db.Transactions.FirstOrDefaultAsync(x => x.InvoiceId == invoice.Id);
                if (transaction == null)
                    return NotFound("Transaction not found for the invoice.");

                db.RemoveRange(transaction, invoice, expence);

                var result = await db.DbSaveChangesAsync();
                return result
                    ? Ok("Expense deleted successfully.")
                    : StatusCode(500, Helper.ErrorInSaveChanges);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }
}
