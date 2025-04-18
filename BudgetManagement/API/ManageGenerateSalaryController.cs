﻿using ExpenseManagment.Data;
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
                var data = await db.GeneratedSallaries
                    .Select(g => new
                    {
                        g.Id,
                        AccName = g.Account.AccName,
                        ProjectName = g.Project.ProjectName,
                        g.BasicAmount,
                        g.BonusAmount,
                        g.GrossPercentAmount,
                        g.GrossTotal,
                        g.InsertionDate,
                        g.GeneratedSalaryMonth,
                        g.AccountId
                    })
                    .ToListAsync();
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
                var existInDb = db.Invoices.Any(x => x.AccountId == model.AccountId);
                if (existInDb != true)
                {
                    var salary = await db.GeneratedSallaries.FindAsync(model.Id);
                    var invoice = new Invoice()
                    {
                        AccountId = salary.AccountId,
                        IsPayable = false,
                        Amount = salary.GrossTotal,
                        //Desc = "Description of the invoice", 
                        InvoiceReffId = salary.ProjectId,
                        InvoiceDate = DateTime.Now,
                        InvoiceType = 4
                    };

                    db.Invoices.Add(invoice);
                    await db.SaveChangesAsync();
                    return Ok("Invoice generated successfully");
                }
                else
                {
                    return Ok("Invoice already!!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [AjaxExceptionFilter]
        [HttpPost("GenerateSalaries")]
        public async Task<IActionResult> GenerateSalaries()
        {
            try
            {
                var monthString = DateTime.Now.ToString("MMM");
                var currentYear = DateTime.Now.ToString("yy");
                var salaryMonth = monthString + "-" + currentYear;

                var existingSalaries = await db.GeneratedSallaries
                    .AnyAsync(s => s.GeneratedSalaryMonth == salaryMonth);

                if (existingSalaries)
                {
                    return Ok("Salaries already");
                }

                List<GeneratedSallary> salaryMappings = new List<GeneratedSallary>();

                var existingMappings = await db.SallaryMappings
                    .Where(s => !s.IsDeleted)
                    .ToListAsync();

                if (existingMappings.Count == 0)
                {
                    return Ok("NoData");
                }

                salaryMappings = existingMappings
                    .Select(s => new GeneratedSallary
                    {
                        AccountId = s.AccountId,
                        ProjectId = s.ProjectId,
                        BasicAmount = s.BasicAmount,
                        BonusAmount = 0,
                        GrossPercentAmount = 0,
                        GrossTotal = 0,
                        GeneratedSalaryMonth = salaryMonth
                    })
                    .ToList();

                db.GeneratedSallaries.AddRange(salaryMappings);

                await db.DbSaveChangesAsync();

                return Ok("Salaries successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("CheckSalariesStatus")]
        public async Task<IActionResult> CheckSalariesStatus()
        {
            try
            {
                var monthString = DateTime.Now.ToString("MMM");
                var currentYear = DateTime.Now.ToString("yy");
                var salaryMonth = monthString + "-" + currentYear;

                var existingSalaries = await db.GeneratedSallaries
                    .AnyAsync(s => s.GeneratedSalaryMonth == salaryMonth);

                if (existingSalaries)
                {
                    return Ok("SalariesGenerated");
                }
                else
                {
                    return Ok("SalariesNotGenerated");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
