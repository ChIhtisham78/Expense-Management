using ExpenseManagment.Data;
using ExpenseManagment.Data.Common;
using ExpenseManagment.Data.DataBaseEntities;
using ExpenseManagment.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagment.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetProjectListController : Controller
    {
        private readonly ApplicationDbContext db;
        public GetProjectListController(ApplicationDbContext _db)
        {
            db = _db;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectList(int id)
        {
            var projectInfo = db.SallaryMappings
                .Where(p => p.Id == id && !p.IsDeleted)
                .Select(p => new { p.ProjectId, p.BasicAmount })
                .FirstOrDefault();

            var effectivePercentProjectMappings = await db.EffectivePercentProjectMappings
                .Where(epm => epm.SallaryMappingId == id)
                .Select(p => p.ProjectId)
                .ToListAsync();

            var concatenatedResult = new List<int> { projectInfo.ProjectId };
            concatenatedResult.AddRange(effectivePercentProjectMappings);

            var listOfInvoices = db.Invoices
                .Where(x => concatenatedResult.Contains(x.InvoiceReffId))
                .GroupBy(x => x.InvoiceReffId)
                .Select(g => g.OrderByDescending(x => x.InvoiceDate).FirstOrDefault())
                .ToList();

            var projectNames = db.Projects
                .Where(proj => concatenatedResult.Contains(proj.Id))
                .Select(proj => new { proj.Id, proj.ProjectName })
                .ToList();

            var accountNames = db.AccountEntities
                .Where(acc => listOfInvoices.Select(x => x.AccountId).Contains(acc.Id))
                .Select(acc => new { acc.Id, acc.AccName })
                .ToList();

            var result = listOfInvoices
                .Join(projectNames,
                    invoice => invoice.InvoiceReffId,
                    projectName => projectName.Id,
                    (invoice, projectName) => new { Invoice = invoice, ProjectName = projectName })
                .Join(accountNames,
                    invoiceProject => invoiceProject.Invoice.AccountId,
                    acc => acc.Id,
                    (invoiceProject, acc) => new
                    {
                        invoiceProject.Invoice.Id,
                        invoiceProject.Invoice.Amount,
                        invoiceProject.Invoice.Desc,
                        invoiceProject.Invoice.InvoiceDate,
                        invoiceProject.ProjectName,
                        acc.AccName,
                        invoiceProject.Invoice.AccountId,
                        ProjectId = invoiceProject.ProjectName.Id,
                        projectInfo.BasicAmount
                    })
                .ToList();

            return Ok(result);
        }

        [AjaxExceptionFilter]
        [HttpPost]
        public async Task<ActionResult> Post(GenerateSalaryViewModel model)
        {
            try
            {
                var monthString = DateTime.Now.ToString("MMM");
                var currentYear = DateTime.Now.ToString("yy");
                var salaryMonth = monthString + "-" + currentYear;

                //// check if basic salaries already generated or not
                var basicSalaries = await db.GeneratedSallaries
                    .AnyAsync(s => s.GeneratedSalaryMonth == salaryMonth);

                if (!basicSalaries)
                {
                    return Ok("Not Add salaries");
                }
                // Get all Generated Salaries on such transaction and current month.
                var existingSalaries = await db.GeneratedSallaries
                    .AnyAsync(s => s.GeneratedSalaryMonth == salaryMonth && s.TransactionId == model.TransactionId);

                if (existingSalaries)
                {
                    return Ok("Salaries already In db");
                }

                // Get Effective Percent
                decimal effectivePercentage = db.Projects.Where(x => x.Id == model.ProjectId).FirstOrDefault().ContractorEffectivePercent;
                // Get Transaction Amount

                var transaction = await db.Invoices.FirstOrDefaultAsync(x => x.Id == model.TransactionId);
                long transactionAmount = transaction?.Amount ?? 0;

                // Get Salary Mapping for getting Basic salary
                var sallaryMapptingsInDb = db.SallaryMappings.Where(x => x.ProjectId == model.ProjectId).AsQueryable();

                long totalEffectiveBasicAmount = sallaryMapptingsInDb.Sum(x => x.BasicAmount);

                var totalEffectivePersonsromeffect = db.EffectivePercentProjectMappings.Where(x => x.ProjectId == model.ProjectId).Count();


                var totalEffectivePersons = totalEffectivePersonsromeffect;

                //if (totalEffectivePersons == 0)
                //{
                //    return Ok("No Contractor");
                //}

                if (totalEffectivePersons > 0)
                {
                    decimal transactionAmountAfterBasic = (transactionAmount - totalEffectiveBasicAmount);

                    decimal totalPercenEffectiveAmount = (transactionAmountAfterBasic / (decimal)100) * effectivePercentage;

                    var perPersonPercentileAmount = (totalPercenEffectiveAmount / totalEffectivePersons);

                    var generatedSallaryList = db.GeneratedSallaries
                        .Where(x => x.ProjectId == model.ProjectId
                                    && x.GeneratedSalaryMonth == salaryMonth
                                    && x.TransactionId == null)
                        .ToList();

                    foreach (var generatedSallary in generatedSallaryList)
                    {

                        var grossPercentAmount = db.GeneratedSallaryPercentiles
                           .Where(x => x.GeneratedSallaryId == generatedSallary.Id)
                           .Sum(x => x.PercentAmount);

                        var generateSalaryInDb = db.GeneratedSallaries.First(x => x.Id == generatedSallary.Id);
                        generateSalaryInDb.GrossPercentAmount = grossPercentAmount;
                        generateSalaryInDb.GrossTotal = (long)(generateSalaryInDb.BasicAmount + generateSalaryInDb.BonusAmount + generateSalaryInDb.GrossPercentAmount);
                        generatedSallary.TransactionId = model.TransactionId;
                        generatedSallary.InsertionDate = DateTime.Now;
                    }

                    var salaryMappingAccountIds = await db.EffectivePercentProjectMappings.Include(x => x.SallaryMapping).Where(x => x.ProjectId == model.ProjectId).
                        Select(x => new { x.SallaryMapping.AccountId }).ToListAsync();

                    var getGeneratedSalaryIds = (from GS in db.GeneratedSallaries.Where(x => x.GeneratedSalaryMonth == salaryMonth).ToList()
                                                 join SA in salaryMappingAccountIds
                                                 on GS.AccountId equals SA.AccountId

                                                 select GS.Id
                                                       ).ToList();

                    foreach (var a in getGeneratedSalaryIds)
                    {
                        GeneratedSallaryPercentile generatedSallaryPercentile = new GeneratedSallaryPercentile();

                        generatedSallaryPercentile.ProjectId = model.ProjectId;
                        generatedSallaryPercentile.GeneratedSallaryId = a;
                        generatedSallaryPercentile.PercentAmount = (long)(perPersonPercentileAmount);
                        db.GeneratedSallaryPercentiles.Add(generatedSallaryPercentile);

                        //wee add GeneratedSallaries update work here we update perPersonPercentileAmount and add to all personss give them 
                        var generatedSallarUpdate = db.GeneratedSallaries.First(x => x.Id == a);
                        generatedSallarUpdate.GrossPercentAmount += (long)(perPersonPercentileAmount);
                        generatedSallarUpdate.GrossTotal = (long)(generatedSallarUpdate.BasicAmount + generatedSallarUpdate.BonusAmount + generatedSallarUpdate.GrossPercentAmount);
                        generatedSallarUpdate.InsertionDate = DateTime.Now;
                        db.GeneratedSallaries.Update(generatedSallarUpdate);
                    }
                    await db.SaveChangesAsync();
                }
                else
                {
                    var salaryMappingsForSingleProject = db.SallaryMappings
                        .Where(x => x.ProjectId == model.ProjectId)
                        .ToList();

                    if (salaryMappingsForSingleProject.Any())
                    {
                        var totalEffectivePer = db.SallaryMappings
                            .Where(x => x.ProjectId == model.ProjectId)
                            .Count();
                        decimal transactionAmountAfterBasic = (transactionAmount - totalEffectiveBasicAmount);
                        decimal totalPercentEffectiveAmount = (transactionAmountAfterBasic / (decimal)100) * effectivePercentage;
                        var perPersonPercentileAmount = (totalPercentEffectiveAmount / totalEffectivePer);
                        var generatedSallaryList = db.GeneratedSallaries
                            .Where(x => x.ProjectId == model.ProjectId
                                        && x.GeneratedSalaryMonth == salaryMonth
                                        && x.TransactionId == null)
                            .ToList();

                        foreach (var generatedSallary in generatedSallaryList)
                        {
                            var generatedSalaryInDb = db.GeneratedSallaries.First(x => x.Id == generatedSallary.Id);

                            // Retrieve PercentAmount from GeneratedSallaryPercentile
                            var percentAmount = db.GeneratedSallaryPercentiles
                                .Where(p => p.GeneratedSallaryId == generatedSallary.Id)
                                .Sum(p => p.PercentAmount);

                            // Update GrossPercentAmount in GeneratedSallary
                            generatedSalaryInDb.GrossPercentAmount = percentAmount;

                            // Calculate GrossTotal based on BasicAmount, BonusAmount, and GrossPercentAmount
                            generatedSalaryInDb.GrossTotal = (long)(generatedSalaryInDb.BasicAmount +
                                                                   (generatedSalaryInDb.BonusAmount ?? 0) +
                                                                   generatedSalaryInDb.GrossPercentAmount);

                            generatedSallary.TransactionId = model.TransactionId;
                            generatedSallary.InsertionDate = DateTime.Now;
                        }

                        await db.SaveChangesAsync();
                    }
                }

                return Ok("Successfully Generated");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}