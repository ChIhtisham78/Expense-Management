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

                if (!ModelState.IsValid)
                {
                    return StatusCode(400, Helper.InvalidModelState);
                }

                var newProject = new Project
                {
                    ClientId = model.ClientId,
                    ProjectName = model.ProjectName,
                    InsertionDate = DateTime.Now,
                    ContractorEffectivePercent = model.ContractorEffectivePercent
                };

                db.Projects.Add(newProject);
                await db.SaveChangesAsync();

                return Ok("Project added successfully");

            }
            catch (Exception exp)
            {
                return StatusCode(500, Helper.ObjectNotFound + exp.Message);
            }
        }

        [HttpGet("Project/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var project = await db.Projects.Include(x => x.Client).FirstOrDefaultAsync(x => x.Id == id);
            return Ok(project);
        }

        [AjaxExceptionFilter]
        [HttpPut("Project")]
        public async Task<IActionResult> EditProject(int id, [FromBody] ProjectModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Helper.InvalidModelState);
            }

            try
            {
                var projectInDb = await db.Projects.FindAsync(id);

                if (projectInDb == null)
                {
                    return NotFound(Helper.ObjectNotFound); 
                }
                projectInDb.ClientId = model.ClientId;
                projectInDb.ProjectName = model.ProjectName;
                projectInDb.ContractorEffectivePercent = model.ContractorEffectivePercent;

                db.Projects.Update(projectInDb);
                await db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        [AjaxExceptionFilter]
        [HttpDelete("DeleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(int id)                                      
        {
            try
            {
                var deleteProject = await db.Projects.FindAsync(id);

                if (deleteProject == null)
                {
                    return NotFound("Project Not Found");
                }

                db.Projects.Remove(deleteProject);

                await db.SaveChangesAsync();

                return Ok("Project Deleted Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
