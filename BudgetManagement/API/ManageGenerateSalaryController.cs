using ExpenseManagment.Custom;
using ExpenseManagment.Data;
using ExpenseManagment.Data.DataBaseEntities;
using ExpenseManagment.Filters;
using ExpenseManagment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagment.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageGenerateSalaryController : Controller
    {
        private readonly ApplicationDbContext db;
        public ManageGenerateSalaryController(ApplicationDbContext _db)
        {
            db = _db;
        }

        [HttpGet]
        public async Task<IActionResult> GetGenerateSalaryData()
        {
            try
            {
                var data = await db.GeneratedSallaries.ToListAsync();
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }



        [AjaxExceptionFilter]
        [HttpPost("GenerateInvoice")]
        public async Task<IActionResult> GenerateInvoice(GenerateInvoiceViewModel model)
        {
            try
            {
                bool invoiceExists = await db.Invoices.AnyAsync(x => x.AccountId == model.AccountId);
                if (invoiceExists)
                    return Conflict("An invoice for this account already exists.");

                var salary = await db.GeneratedSallaries.FindAsync(model.Id);
                if (salary == null)
                    return NotFound("Salary data not found for the provided ID.");

                var invoice = new Invoice
                {
                    AccountId = salary.AccountId,
                    IsPayable = false,
                    Amount = salary.GrossTotal,
                    InvoiceReffId = salary.ProjectId,
                    InvoiceDate = DateTime.UtcNow,
                    InvoiceType = (int)Helper.InvoiceTypeId.SallaryInvoice
                };

                await db.Invoices.AddAsync(invoice);
                await db.SaveChangesAsync();

                return Ok("Invoice generated successfully.");
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                return StatusCode(500, $"An error occurred while generating invoice: {ex.Message}");
            }
        }


        [AjaxExceptionFilter]
        [HttpPost("GenerateSalaries")]
        public async Task<IActionResult> GenerateSalaries()
        {
            try
            {
                var currentMonth = DateTime.Now.ToString("MMM");
                var currentYear = DateTime.Now.ToString("yy");
                var salaryMonth = $"{currentMonth}-{currentYear}";

                var salariesExist = await db.GeneratedSallaries
                    .AnyAsync(s => s.GeneratedSalaryMonth == salaryMonth);

                if (salariesExist)
                {
                    return Ok("Salaries already generated.");
                }

                var salaryMappings = await db.SallaryMappings
                    .Where(s => !s.IsDeleted)
                    .ToListAsync();

                if (!salaryMappings.Any())
                {
                    return Ok("No salary mappings found.");
                }

                var generatedSalaries = salaryMappings.Select(s => new GeneratedSallary
                {
                    AccountId = s.AccountId,
                    ProjectId = s.ProjectId,
                    BasicAmount = s.BasicAmount,
                    BonusAmount = 0,
                    GrossPercentAmount = 0,
                    GrossTotal = 0,
                    GeneratedSalaryMonth = salaryMonth
                }).ToList();

                await db.GeneratedSallaries.AddRangeAsync(generatedSalaries);
                await db.DbSaveChangesAsync();

                return Ok("Salaries generated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("CheckSalariesStatus")]
        public async Task<IActionResult> CheckSalariesStatus()
        {
            try
            {
                var currentMonth = DateTime.Now.ToString("MMM");
                var currentYear = DateTime.Now.ToString("yy");
                var salaryMonth = $"{currentMonth}-{currentYear}";

                var salariesGenerated = await db.GeneratedSallaries
                    .AnyAsync(s => s.GeneratedSalaryMonth == salaryMonth);

                return Ok(salariesGenerated ? "SalariesGenerated" : "SalariesNotGenerated");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }
}
