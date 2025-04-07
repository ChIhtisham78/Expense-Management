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
            return Ok(await db.Expences.Include(x => x.Account).ToListAsync());
        }


        //[HttpGet("DeletedExpences")]
        //public async Task<IActionResult> GetDeletedExpences()
        //{
        //    return Ok(await db.DeletedExpences.Include(x => x.Account).ToListAsync());
        //}

        [HttpGet("Expence/{id}")]
        public async Task<IActionResult> GetExpences(int id)
        {
            return Ok(await db.Expences.Include(x => x.Account).FirstOrDefaultAsync(x => x.Id == id));
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

            try
            {
                if (ModelState.IsValid)
                {
                    int expenceId = model.Id;
                    Expence expence = await db.Expences.FirstOrDefaultAsync(x => x.Id == expenceId);

                    Invoice invoice = await db.Invoices.FirstOrDefaultAsync(x => x.InvoiceReffId == expenceId && x.InvoiceType == (int)Helper.InvoiceTypeId.ExpenceFromBusinessAccount);
                    if (expence != null && invoice != null)
                    {
                        Transaction transaction = await db.Transactions.FirstOrDefaultAsync(x => x.InvoiceId == invoice.Id);

                        if (transaction != null)
                        {
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

                            db.Entry(expence).State = EntityState.Modified;
                            db.Entry(invoice).State = EntityState.Modified;
                            db.Entry(transaction).State = EntityState.Modified;
                            if (await db.DbSaveChangesAsync())
                            {
                                return Ok();
                            }
                            return StatusCode(500, Helper.ErrorInSaveChanges);
                        }
                        return StatusCode(500, Helper.ObjectNotFound);
                    }
                    return StatusCode(500, Helper.InvalidModelState);
                }
            }
            catch (Exception exp)
            {
                return StatusCode(500, Helper.ObjectNotFound + exp.Message);
            }
            return BadRequest();
        }
        [AjaxExceptionFilter]
        [HttpDelete("DeleteExpence/{id}")]
        public async Task<IActionResult> DeleteExpence(int id)
        {

            try
            {
                Expence expence = await db.Expences.FirstOrDefaultAsync(x => x.Id == id);
                Invoice invoice = await db.Invoices.FirstOrDefaultAsync(x => x.InvoiceReffId == id && x.InvoiceType == (int)Helper.InvoiceTypeId.ExpenceFromBusinessAccount);
                if (expence != null && invoice != null)
                {
                    Transaction transaction = await db.Transactions.FirstOrDefaultAsync(x => x.InvoiceId == invoice.Id);
                    //DeletedExpence deletedExpence = new DeletedExpence();
                    //deletedExpence.ExpenceType = expence.ExpenceType;
                    //deletedExpence.ExpenceDate = expence.ExpenceDate;
                    //deletedExpence.ExpenceDesc = expence.ExpenceDesc;
                    //deletedExpence.ExpenceAmount = expence.ExpenceAmount;
                    //deletedExpence.InsertionDate = expence.InsertionDate;
                    //deletedExpence.AccountId = expence.AccountId;
                    //deletedExpence.DeletedBy = User.Identity.Name;
                    //deletedExpence.DeletedOn = DateTime.Now;

                    //db.DeletedExpences.Add(deletedExpence);

                    db.Transactions.Remove(transaction);
                    db.Invoices.Remove(invoice);
                    db.Expences.Remove(expence);

                    if (await db.DbSaveChangesAsync())
                    {
                        return Ok();
                    }
                    return StatusCode(500, Helper.ErrorInSaveChanges);
                }
                return StatusCode(500, Helper.ObjectNotFound);
            }
            catch (Exception exp)
            {
                return StatusCode(500, Helper.ObjectNotFound + exp.Message);
            }
        }

    }
}
