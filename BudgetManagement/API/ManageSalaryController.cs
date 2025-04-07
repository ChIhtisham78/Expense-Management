using ExpenseManagment.Custom;
using ExpenseManagment.Data;
using ExpenseManagment.Data.Common;
using ExpenseManagment.Data.DataBaseEntities;
using ExpenseManagment.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseManagment.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageSalaryController : BaseController
    {
        private readonly ApplicationDbContext db;

        public ManageSalaryController(ApplicationDbContext _db)
        {
            db = _db;
        }

        [HttpGet]
        [Authorize(Roles = Helper.RolesAttrVal.Salary)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var resultSingle = await (from sallary in db.SallaryMappings
                                          join Proj in db.Projects
                                          on sallary.ProjectId equals Proj.Id
                                          join acc in db.AccountEntities
                                          on sallary.AccountId equals acc.Id
                                          where !sallary.IsDeleted
                                          select new SalaryModel
                                          {
                                              Id = sallary.Id,
                                              AccountName = acc.AccName,
                                              SingleProject = Proj.ProjectName,
                                              BasicAmount = sallary.BasicAmount,
                                              AccountId = sallary.AccountId,
                                              SingleProjectId = sallary.ProjectId,
                                              EffectivePercentProjectMappings = (from epm in db.EffectivePercentProjectMappings
                                                                                 where epm.SallaryMappingId == sallary.Id
                                                                                 select new EffectivePercentProjectMappingModel
                                                                                 {
                                                                                     Id = epm.Id,
                                                                                     ProjectId = epm.ProjectId,
                                                                                 }).ToList()

                                          }).ToListAsync();

                foreach (var salaryModel in resultSingle)
                {
                    foreach (var epm in salaryModel.EffectivePercentProjectMappings)
                    {
                        var project = db.Projects.FirstOrDefault(p => p.Id == epm.ProjectId);
                        if (project != null)
                        {
                            epm.ProjectName = project.ProjectName;
                        }
                    }
                }
                return Ok(resultSingle);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        [HttpGet("GetAccountIds")]
        public async Task<IActionResult> GetAccountIds()
        {
            try
            {
                var accountIds = await db.GeneratedSallaries.Select(x => x.AccountId).ToListAsync();
                return Ok(accountIds);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var accountSallaryMappings = await db.SallaryMappings.Where(td => td.Id == id)
                    .Include(x => x.Project)
                    .Include(x => x.Account)
                    .FirstOrDefaultAsync();

                var effectivePercentProjectMappings = await db.EffectivePercentProjectMappings
                .Where(epm => epm.SallaryMappingId == id)
                .ToListAsync();

                var result = new
                {
                    SallaryMapping = accountSallaryMappings,
                    EffectivePercentProjectMappings = effectivePercentProjectMappings
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        [AjaxExceptionFilter]
        [HttpPost]
        public async Task<IActionResult> Post(SalaryModel model)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.SallaryMappings.FirstOrDefault(x => x.AccountId == model.AccountId && !x.IsDeleted) == null)
                    {
                        SallaryMapping objsallary = new SallaryMapping
                        {
                            AccountId = model.AccountId,
                            ProjectId = model.SingleProjectId,
                            InsertionDate = DateTime.Now,
                            BasicAmount = model.BasicAmount
                        };
                        db.SallaryMappings.Add(objsallary);
                        await db.DbSaveChangesAsync();

                        if (!string.IsNullOrEmpty(model.MultipleProjectId))
                        {
                            int[] intArray = model.MultipleProjectId.Split(',').Select(int.Parse).ToArray();
                            foreach (var item in intArray)
                            {
                                EffectivePercentProjectMapping obj = new EffectivePercentProjectMapping
                                {
                                    SallaryMappingId = objsallary.Id,
                                    ProjectId = item,
                                };
                                db.EffectivePercentProjectMappings.Add(obj);
                            }
                        }

                        if (await db.DbSaveChangesAsync())
                        {
                            return Ok();
                        }
                        return StatusCode(500, Helper.ErrorInSaveChanges);
                    }
                    else
                    {
                        return ResponseResult(true, message: "Already Exists");
                    }
                }
                return StatusCode(500, Helper.InvalidModelState);
            }
            catch (Exception exp)
            {
                return StatusCode(500, Helper.ObjectNotFound + exp.Message);
            }
        }

        [AjaxExceptionFilter]
        [HttpPut]
        public async Task<IActionResult> Put(SalaryModel model)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.SallaryMappings.FirstOrDefault(x => x.AccountId == model.AccountId && x.Id != model.Id && !x.IsDeleted) == null)
                    {
                        var existingSalary = await db.SallaryMappings.FindAsync(model.Id);

                        if (existingSalary == null)
                        {
                            return NotFound(Helper.ObjectNotFound);
                        }

                        existingSalary.AccountId = model.AccountId;
                        existingSalary.ProjectId = model.SingleProjectId;
                        existingSalary.BasicAmount = model.BasicAmount;

                        var existingMappings = db.EffectivePercentProjectMappings.Where(epm => epm.SallaryMappingId == existingSalary.Id);
                        db.EffectivePercentProjectMappings.RemoveRange(existingMappings);

                        if (!string.IsNullOrEmpty(model.MultipleProjectId))
                        {
                            int[] intArray = model.MultipleProjectId.Split(',').Select(int.Parse).ToArray();

                            foreach (var item in intArray)
                            {
                                EffectivePercentProjectMapping obj = new EffectivePercentProjectMapping
                                {
                                    SallaryMappingId = existingSalary.Id,
                                    ProjectId = item,
                                };
                                db.EffectivePercentProjectMappings.Add(obj);
                            }
                        }

                        if (await db.DbSaveChangesAsync())
                        {
                            return Ok();
                        }
                        return StatusCode(500, Helper.ErrorInSaveChanges);
                    }
                    else
                    {
                        return ResponseResult(true, message: "Already Exists");
                    }
                }
                return StatusCode(500, Helper.InvalidModelState);
            }
            catch (Exception exp)
            {
                return StatusCode(500, Helper.ObjectNotFound + exp.Message);
            }
        }

        [AjaxExceptionFilter]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            
            try
            {
                var obj = await db.SallaryMappings.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (obj != null)
                {
                    obj.IsDeleted = true;
                    await db.DbSaveChangesAsync();
                }
                var obj2 = await db.EffectivePercentProjectMappings.Where(x => x.SallaryMappingId == obj.Id).ToListAsync();
                if (obj2 != null)
                {
                    db.EffectivePercentProjectMappings.RemoveRange(obj2);
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
