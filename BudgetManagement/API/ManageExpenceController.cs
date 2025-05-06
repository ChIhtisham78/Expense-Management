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

        [HttpGet("Expence/{id}")]
        public async Task<IActionResult> GetExpences(int id)
        {
            var expense = await db.Expences
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (expense == null)
            {
                return NotFound($"Expense with ID {id} was not found.");
            }

            return Ok(expense);
        }

        [AjaxExceptionFilter]
        [HttpPost("Expence")]

        [HttpPost("PostExpence")]
        public async Task<IActionResult> PostExpence(ExpenceModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(400, Helper.InvalidModelState);
                }

                var defaultExpenseAccountId = db.AccountEntities
                    .Single(x => x.AccountTypeId == (int)Helper.AccountTypeId.GeneralExpenceAccountOfBusiness)
                    .Id;

                var newExpense = new Expence
                {
                    ExpenceType = model.ExpenceType,
                    ExpenceDate = model.ExpenceDate,
                    ExpenceDesc = model.ExpenceDesc,
                    AccountId = model.AccountId,
                    ExpenceAmount = model.ExpenceAmount,
                    InsertionDate = DateTime.Now
                };

                db.Expences.Add(newExpense);
                await db.DbSaveChangesAsync();

                var invoice = new Invoice
                {
                    AccountId = defaultExpenseAccountId,
                    IsPayable = true,
                    Amount = newExpense.ExpenceAmount,
                    Desc = newExpense.ExpenceDesc,
                    InvoiceReffId = newExpense.Id,
                    InvoiceDate = newExpense.ExpenceDate,
                    InvoiceType = (int)Helper.InvoiceTypeId.ExpenceFromBusinessAccount
                };

                db.Invoices.Add(invoice);
                await db.DbSaveChangesAsync();

                var transaction = new Transaction
                {
                    InvoiceId = invoice.Id,
                    Amount = newExpense.ExpenceAmount,
                    Desc = newExpense.ExpenceDesc,
                    TransactionDate = newExpense.ExpenceDate,
                    BeneficiaryAccountId = newExpense.AccountId,
                    CreateOn = DateTime.Now,
                    CreatedBy = User.Identity?.Name
                };

                db.Transactions.Add(transaction);

                if (await db.DbSaveChangesAsync())
                {
                    return Ok();
                }

                return StatusCode(500, Helper.ErrorInSaveChanges);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{Helper.ObjectNotFound}: {ex.Message}");
            }
        }


        [AjaxExceptionFilter]
        [HttpPut("Expence")]
        public async Task<IActionResult> PutExpence(ExpenceModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data submitted.");
            }

            try
            {
                var expense = await db.Expences.FirstOrDefaultAsync(x => x.Id == model.Id);
                var invoice = await db.Invoices
                    .FirstOrDefaultAsync(x => x.InvoiceReffId == model.Id &&
                                              x.InvoiceType == (int)Helper.InvoiceTypeId.ExpenceFromBusinessAccount);

                if (expense == null || invoice == null)
                {
                    return NotFound("Expense or related invoice not found.");
                }

                var transaction = await db.Transactions.FirstOrDefaultAsync(x => x.InvoiceId == invoice.Id);
                if (transaction == null)
                {
                    return NotFound("Transaction not found.");
                }

                expense.ExpenceType = model.ExpenceType;
                expense.ExpenceDate = model.ExpenceDate;
                expense.ExpenceDesc = model.ExpenceDesc;
                expense.ExpenceAmount = model.ExpenceAmount;
                expense.AccountId = model.AccountId;

                invoice.Amount = model.ExpenceAmount;
                invoice.Desc = model.ExpenceDesc;
                invoice.InvoiceDate = model.ExpenceDate;

                transaction.Amount = model.ExpenceAmount;
                transaction.Desc = model.ExpenceDesc;
                transaction.TransactionDate = model.ExpenceDate;
                transaction.BeneficiaryAccountId = model.AccountId;

                db.UpdateRange(expense, invoice, transaction);

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

        [ValidateAntiForgeryToken]
        [AjaxExceptionFilter]
        [HttpDelete("DeleteExpence/{id}")]
        public async Task<IActionResult> DeleteExpence(int id)
        {
            try
            {
                var expense = await db.Expences.FirstOrDefaultAsync(x => x.Id == id);
                if (expense == null)
                {
                    return NotFound("Expense not found.");
                }

                var invoice = await db.Invoices
                    .FirstOrDefaultAsync(x => x.InvoiceReffId == id &&
                                              x.InvoiceType == (int)Helper.InvoiceTypeId.ExpenceFromBusinessAccount);
                if (invoice == null)
                {
                    return NotFound("Invoice not found for the expense.");
                }

                var transaction = await db.Transactions
                    .FirstOrDefaultAsync(x => x.InvoiceId == invoice.Id);
                if (transaction == null)
                {
                    return NotFound("Transaction not found for the invoice.");
                }

                db.RemoveRange(transaction, invoice, expense);

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
