using ConstructionAccSystem.Data.Common;
using ExpenseManagment.Custom;
using ExpenseManagment.Data;
using ExpenseManagment.Data.DataBaseEntities;
using ExpenseManagment.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagment.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageAccountController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public ManageAccountController(ApplicationDbContext _db)
        {
            db = _db;
        }

        [HttpGet("Account/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var getAccount = await db.AccountEntities.FindAsync(id);
            return Ok(getAccount);
        }

        [AjaxExceptionFilter]
        [HttpPut("EditAccount")]
        public async Task<IActionResult> EditClient(AccountModel model)
        {
            try
            {
                var clientInDb = await db.AccountEntities.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (clientInDb == null)
                    return NotFound();

                clientInDb.AccName = model.Name;
                clientInDb.Email = model.Email;
                clientInDb.Cell = model.Cell;
                clientInDb.WebSiteLink = model.WebSiteLink;
                clientInDb.Desc = model.Desc;
                await db.DbSaveChangesAsync();

                return Ok(clientInDb);

            }
            catch (Exception exp)
            {
                return StatusCode(500, Helper.ObjectNotFound + exp.Message);
            }
        }
        [AjaxExceptionFilter]
        [HttpDelete("DeleteAccount/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                var deleteAccount = await db.AccountEntities.FindAsync(id);

                if (deleteAccount == null)
                    return NotFound("Account not found.");

                db.AccountEntities.Remove(deleteAccount);
                await db.DbSaveChangesAsync();

                return Ok("Account deleted successfully.");
            }
            catch (Exception exp)
            {
                return StatusCode(500, Helper.ObjectNotFound + exp.Message);
            }
        }



        // Get : Clients

        #region

        [HttpGet("Clients")]
        [Authorize(Roles = Helper.RolesAttrVal.Client)]
        public async Task<IActionResult> GetClients()
        {
            var getClients = await db.AccountEntities.Where(x=>x.AccountTypeId==(int)Helper.AccountTypeId.client).OrderByDescending(x => x.Id).ToListAsync();
            return Ok(getClients);
        }

        // Post : Client
        [AjaxExceptionFilter]
        [HttpPost("Client")]
        public async Task<IActionResult> AddNewClient(AccountModel model)
        {


            try
            {
                if (ModelState.IsValid)
                {
                    AccountEntity client = new AccountEntity();
                    client.AccName = model.Name;
                    client.Email = model.Email;
                    client.Cell = model.Cell;
                    client.WebSiteLink = model.WebSiteLink;
                    client.Desc = model.Desc;
                    client.AccountTypeId = (int)Helper.AccountTypeId.client;
                    client.CreationDate = DateTime.Now;
                    db.AccountEntities.Add(client);
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

        #endregion

        // Get : Business
        #region

        [HttpGet("Business")]
        [Authorize(Roles = Helper.RolesAttrVal.Business)]
        public async Task<IActionResult> GetBusiness()
        {
            return Ok(await db.AccountEntities.Where(x => x.AccountTypeId == (int)Helper.AccountTypeId.business).OrderByDescending(x => x.Id).ToListAsync());
        }

        // Post : Business
        [AjaxExceptionFilter]
        [HttpPost("Business")]
        public async Task<IActionResult> AddNewBusiness(AccountModel model)
        {


            try
            {
                if (ModelState.IsValid)
                {
                    AccountEntity business = new AccountEntity();
                    business.AccName = model.Name;
                    business.Email = model.Email;
                    business.Cell = model.Cell;
                    business.WebSiteLink = model.WebSiteLink;
                    business.Desc = model.Desc;
                    business.AccountTypeId = (int)Helper.AccountTypeId.business;
                    business.CreationDate = DateTime.Now;
                    db.AccountEntities.Add(business);
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

        #endregion

        #region

        // Get : Partner

        [HttpGet("Partner")]
        [Authorize(Roles = Helper.RolesAttrVal.Partner)]
        public async Task<IActionResult> GetPartner()
        {
            return Ok(await db.AccountEntities.Where(x => x.AccountTypeId == (int)Helper.AccountTypeId.partner).OrderByDescending(x => x.Id).ToListAsync());
        }

        // Post : Partner
        [AjaxExceptionFilter]
        [HttpPost("Partner")]
        public async Task<IActionResult> AddNewPartner(AccountModel model)
        {


            try
            {
                if (ModelState.IsValid)
                {
                    AccountEntity partner = new AccountEntity();
                    partner.AccName = model.Name;
                    partner.Email = model.Email;
                    partner.Cell = model.Cell;
                    partner.WebSiteLink = model.WebSiteLink;
                    partner.Desc = model.Desc;
                    partner.AccountTypeId = (int)Helper.AccountTypeId.partner;
                    partner.CreationDate = DateTime.Now;
                    db.AccountEntities.Add(partner);
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

        #endregion

        #region

        // Get : Contractor
        [HttpGet("Contractor")]
        [Authorize(Roles = Helper.RolesAttrVal.Contractor)]
        public async Task<IActionResult> GetContactor()
        {
            return Ok(await db.AccountEntities.Where(x => x.AccountTypeId == (int)Helper.AccountTypeId.contractor).OrderByDescending(x => x.Id).ToListAsync());
        }

        // Post : Contractor
        [AjaxExceptionFilter]
        [HttpPost("Contractor")]
        public async Task<IActionResult> AddNewContractor(AccountModel model)
        {


            try
            {
                if (ModelState.IsValid)
                {
                    AccountEntity contractor = new AccountEntity();
                    contractor.AccName = model.Name;
                    contractor.Email = model.Email;
                    contractor.Cell = model.Cell;
                    contractor.WebSiteLink = model.WebSiteLink;
                    contractor.Desc = model.Desc;
                    contractor.AccountTypeId = (int)Helper.AccountTypeId.contractor;
                    contractor.CreationDate = DateTime.Now;
                    db.AccountEntities.Add(contractor);
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

        #endregion
    }
}
