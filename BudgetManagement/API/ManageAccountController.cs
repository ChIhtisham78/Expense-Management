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



        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return NotFound("File Not Found");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { filePath });

        }


        // Get : Clients

        #region

        [HttpGet("Clients")]
        [Authorize(Roles = Helper.RolesAttrVal.Client)]
        public async Task<IActionResult> GetClients()
        {
            var getClients = await db.AccountEntities.Where(x => x.AccountTypeId == (int)Helper.AccountTypeId.client).OrderByDescending(x => x.Id).ToListAsync();
            return Ok(getClients);
        }

        // Post : Client
        [AjaxExceptionFilter]
        [HttpPost("Client")]
        public async Task<IActionResult> AddNewClient(AccountModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(400, Helper.InvalidModelState);
                }

                var client = new AccountEntity
                {
                    AccName = model.Name,
                    Email = model.Email,
                    Cell = model.Cell,
                    WebSiteLink = model.WebSiteLink,
                    Desc = model.Desc,
                    AccountTypeId = (int)Helper.AccountTypeId.client,
                    CreationDate = DateTime.Now
                };

                db.AccountEntities.Add(client);

                bool isSaved = await db.DbSaveChangesAsync();
                if (isSaved)
                {
                    return Ok();
                }

                return StatusCode(500, Helper.ErrorInSaveChanges);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(Helper.InvalidModelState);
            }

            try
            {
                var businessEntity = new AccountEntity
                {
                    AccName = model.Name,
                    Email = model.Email,
                    Cell = model.Cell,
                    WebSiteLink = model.WebSiteLink,
                    Desc = model.Desc,
                    AccountTypeId = (int)Helper.AccountTypeId.business,
                    CreationDate = DateTime.UtcNow
                };

                db.AccountEntities.Add(businessEntity);

                var saveResult = await db.DbSaveChangesAsync();
                if (saveResult)
                {
                    return Ok();
                }

                return StatusCode(500, Helper.ErrorInSaveChanges);
            }
            catch (Exception ex)
            {
                // Optionally log the exception here using your logging framework
                return StatusCode(500, $"{Helper.ObjectNotFound} {ex.Message}");
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
            if (!ModelState.IsValid)
            {
                return BadRequest(Helper.InvalidModelState);
            }

            try
            {
                var partnerEntity = new AccountEntity
                {
                    AccName = model.Name,
                    Email = model.Email,
                    Cell = model.Cell,
                    WebSiteLink = model.WebSiteLink,
                    Desc = model.Desc,
                    AccountTypeId = (int)Helper.AccountTypeId.partner,
                    CreationDate = DateTime.UtcNow
                };

                db.AccountEntities.Add(partnerEntity);

                var saveResult = await db.DbSaveChangesAsync();
                if (saveResult)
                {
                    return Ok();
                }

                return StatusCode(500, Helper.ErrorInSaveChanges);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{Helper.ObjectNotFound} {ex.Message}");
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
            if (!ModelState.IsValid)
            {
                return BadRequest(Helper.InvalidModelState);
            }

            try
            {
                var contractorEntity = new AccountEntity
                {
                    AccName = model.Name,
                    Email = model.Email,
                    Cell = model.Cell,
                    WebSiteLink = model.WebSiteLink,
                    Desc = model.Desc,
                    AccountTypeId = (int)Helper.AccountTypeId.contractor,
                    CreationDate = DateTime.UtcNow
                };

                db.AccountEntities.Add(contractorEntity);

                var saveResult = await db.DbSaveChangesAsync();
                if (saveResult)
                {
                    return Ok();
                }

                return StatusCode(500, Helper.ErrorInSaveChanges);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{Helper.ObjectNotFound} {ex.Message}");
            }
        }


        #endregion
    }
}
