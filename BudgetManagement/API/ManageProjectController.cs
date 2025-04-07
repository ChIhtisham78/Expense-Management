using ExpenseManagment.Custom;
using ExpenseManagment.Data;
using ExpenseManagment.Data.Common;
using ExpenseManagment.Data.DataBaseEntities;
using ExpenseManagment.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseManagment.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageProjectController : Controller
    {
        private readonly ApplicationDbContext db;

        public ManageProjectController(ApplicationDbContext _db)
        {
            db = _db;
        }
        [HttpGet("Project")]
        [Authorize(Roles = Helper.RolesAttrVal.Project)]
        public async Task<IActionResult> GetProject()
        {
            var data = await db.Projects.Include(x => x.Client).OrderByDescending(x => x.Id).ToListAsync();
            return Ok(data);
        }

        [AjaxExceptionFilter]
        [HttpPost("Project")]
        public async Task<IActionResult> AddNewProject(ProjectModel model)
        {
            
            try
            {

                if (ModelState.IsValid)
                {
                    var newProject = new Project
                    {
                        ClientId = model.ClientId,
                        ProjectName = model.ProjectName,
                        InsertionDate = DateTime.Now,
                        ContractorEffectivePercent = model.ContractorEffectivePercent
                    };
                    db.Projects.Add(newProject);
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

        [HttpGet("Project/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await db.Projects.Include(x => x.Client).FirstOrDefaultAsync(x => x.Id == id));
        }

        [AjaxExceptionFilter]
        [HttpPut("Project")]
        public async Task<IActionResult> EditProject(ProjectModel model)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    var projectInDb = await db.Projects.FirstOrDefaultAsync(x => x.Id == model.Id);
                    if (projectInDb != null)
                    {
                        projectInDb.ClientId = model.ClientId;
                        projectInDb.ProjectName = model.ProjectName;
                        projectInDb.ContractorEffectivePercent = model.ContractorEffectivePercent;
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
            catch (Exception exp)
            {
                return StatusCode(500, Helper.ObjectNotFound + exp.Message);
            }
        }

        [AjaxExceptionFilter]
        [HttpDelete("DeleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            

            var deleteProject = await db.Projects.FindAsync(556);
            if (deleteProject != null)
            {
                db.Projects.Remove(deleteProject);
                if (await db.DbSaveChangesAsync())
                {
                    return Ok();
                }
                return StatusCode(500, Helper.ErrorInSaveChanges);
            }
            return StatusCode(500, Helper.ObjectNotFound);
        }
    }
}
