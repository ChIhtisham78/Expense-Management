using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManagment.Data.DataBaseEntities
{
    public class AccountEntity
    {
        //everything have individual Account
        [Key]
        public int Id { get; set; }
        [Required]
        public int AccountTypeId { get; set; }// enum for this: client = 1, business = 2, partner = 3, contractor = 4, GeneralExpenceAccountOfBusiness = 5, BusinessCapitalAccount = 6
        [Required]
        public string AccName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cell { get; set; } = string.Empty;
        public string WebSiteLink { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
    }

    public class Project
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        [Required]
        public string ProjectName { get; set; } = string.Empty;
        public DateTime InsertionDate { get; set; }
        public decimal ContractorEffectivePercent { get; set; }// should have 2.00 digit round value less then 100 && grater then 0.

        [ForeignKey("ClientId")]
        public virtual AccountEntity Client { get; set; } /*= new AccountEntity();*/

    }

    public class Expence
    {
        [Key]
        public int Id { get; set; }
        public string ExpenceType { get; set; }
        public DateTime ExpenceDate { get; set; }
        public string ExpenceDesc { get; set; }
        public long ExpenceAmount { get; set; }
        public DateTime InsertionDate { get; set; }
        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual AccountEntity Account { get; set; }
    }

    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public bool IsPayable { get; set; }
        public long Amount { get; set; }
        public string Desc { get; set; } = String.Empty;
        public int InvoiceReffId { get; set; } // 0 for the capital transaction , by InvoiceReffId we know that the any Invoice is againt which service Invoice 
        public DateTime InvoiceDate { get; set; }
        public int InvoiceType { get; set; } // 1 for Project Milestone , 2 for Contractor Milestone , 3 for purchase , 4 for  ExpenceId

        [ForeignKey("AccountId")]
        public virtual AccountEntity AccountEntities { get; set; }
    }

    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public long Amount { get; set; }
        public int BeneficiaryAccountId { get; set; }
        public string Desc { get; set; } = String.Empty;
        public DateTime TransactionDate { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
        public string CreatedBy { get; set; } = String.Empty; // put the name of logedin person
        public DateTime CreateOn { get; set; } // Need to put the datetime.now

    }

    public class SallaryMapping
    {
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; } // Contractor Account Id
        [ForeignKey("AccountId")]
        public virtual AccountEntity Account { get; set; }
        public int ProjectId { get; set; } // this project only effect basic 
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public long BasicAmount { get; set; }
        public DateTime InsertionDate { get; set; } // system date
        public bool IsDeleted { get; set; } // only one AccountId should be active at the same time.

    }

    public class EffectivePercentProjectMapping
    {
        [Key]
        public int Id { get; set; }
        public int SallaryMappingId { get; set; } // Contractor Account Id
        [ForeignKey("SallaryMappingId")]
        public virtual SallaryMapping SallaryMapping { get; set; }
        public int ProjectId { get; set; } // Project Id
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

    }

    public class GeneratedSallary
    {
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; } // Contractor Account Id
        [ForeignKey("AccountId")]
        public virtual AccountEntity Account { get; set; }
        public int ProjectId { get; set; } // Project Id that effect Basic single select 
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public long BasicAmount { get; set; } // not required on UI
        public long? BonusAmount { get; set; } // we will add bonus latter nullable 
        public long GrossPercentAmount { get; set; } // 0 for now 
        public long GrossTotal { get; set; } // 0 for now 
        public int? TransactionId { get; set; } = null;
        public string GeneratedSalaryMonth { get; set; } // Datetime 
        public DateTime InsertionDate { get; set; } // system date

    }
    public class GeneratedSallaryPercentile
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public int GeneratedSallaryId { get; set; }
        [ForeignKey("GeneratedSallaryId")]
        public virtual GeneratedSallary GeneratedSallary { get; set; }
        public long PercentAmount { get; set; }
    }
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Password { get; set; }
        public bool IsCompleted { get; set; }
        public string ProfilePicturePath { get; set; }
        public string ZipCode { get; set; }
    }
}