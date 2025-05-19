using ExpenseManagment.Custom;
using ExpenseManagment.Data;
using ExpenseManagment.Data.Common;
using ExpenseManagment.Data.DataBaseEntities;
using ExpenseManagment.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
                if (!ModelState.IsValid)
                {
                    return StatusCode(500, Helper.InvalidModelState);
                }

                var existingSalary = db.SallaryMappings
                    .FirstOrDefault(x => x.AccountId == model.AccountId && !x.IsDeleted);

                if (existingSalary != null)
                {
                    return ResponseResult(true, message: "Already Exists");
                }

                var salaryMapping = new SallaryMapping
                {
                    AccountId = model.AccountId,
                    ProjectId = model.SingleProjectId,
                    InsertionDate = DateTime.UtcNow,
                    BasicAmount = model.BasicAmount
                };

                db.SallaryMappings.Add(salaryMapping);
                await db.DbSaveChangesAsync();

                if (!string.IsNullOrEmpty(model.MultipleProjectId))
                {
                    var projectIds = model.MultipleProjectId
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => int.Parse(id.Trim()));

                    foreach (var projectId in projectIds)
                    {
                        var effectiveMapping = new EffectivePercentProjectMapping
                        {
                            SallaryMappingId = salaryMapping.Id,
                            ProjectId = projectId
                        };
                        db.EffectivePercentProjectMappings.Add(effectiveMapping);
                    }
                }

                var saveResult = await db.DbSaveChangesAsync();
                return Ok(saveResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{Helper.ObjectNotFound}{ex.Message}");
            }
        }


        [AjaxExceptionFilter]
        [HttpPut]
        public async Task<IActionResult> Put(SalaryModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(500, Helper.InvalidModelState);
                }

                var duplicateSalary = db.SallaryMappings
                    .FirstOrDefault(x => x.AccountId == model.AccountId && x.Id != model.Id && !x.IsDeleted);

                if (duplicateSalary != null)
                {
                    return ResponseResult(true, message: "Already Exists");
                }

                var existingSalary = await db.SallaryMappings.FindAsync(model.Id);
                if (existingSalary == null)
                {
                    return NotFound(Helper.ObjectNotFound);
                }

                existingSalary.AccountId = model.AccountId;
                existingSalary.ProjectId = model.SingleProjectId;
                existingSalary.BasicAmount = model.BasicAmount;

                var existingMappings = db.EffectivePercentProjectMappings
                    .Where(epm => epm.SallaryMappingId == existingSalary.Id)
                    .ToList();

                db.EffectivePercentProjectMappings.RemoveRange(existingMappings);

                if (!string.IsNullOrEmpty(model.MultipleProjectId))
                {
                    var projectIds = model.MultipleProjectId
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => int.Parse(id.Trim()));

                    foreach (var projectId in projectIds)
                    {
                        var newMapping = new EffectivePercentProjectMapping
                        {
                            SallaryMappingId = existingSalary.Id,
                            ProjectId = projectId
                        };
                        db.EffectivePercentProjectMappings.Add(newMapping);
                    }
                }

                var saveResult = await db.DbSaveChangesAsync();
                
                return Ok(saveResult);
                            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{Helper.ObjectNotFound}{ex.Message}");
            }
        }


        [AjaxExceptionFilter]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var salaryMapping = await db.SallaryMappings.FindAsync(id);

                if (salaryMapping == null)
                {
                    return NotFound(Helper.ObjectNotFound);
                }

                salaryMapping.IsDeleted = true;
                await db.DbSaveChangesAsync();

                var relatedMappings = await db.EffectivePercentProjectMappings
                    .Where(x => x.SallaryMappingId == id)
                    .ToListAsync();

                if (relatedMappings.Any())
                {
                   db.EffectivePercentProjectMappings.RemoveRange(relatedMappings);

                    var saveResult = await db.DbSaveChangesAsync();
                    if (!saveResult)
                    {
                        return StatusCode(500, Helper.ErrorInSaveChanges);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{Helper.ErrorInSaveChanges}: {ex.Message}");
            }
        }


    }

}
