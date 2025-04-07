
namespace ExpenseManagment.Data.Common
{
        public class SalaryModel
        {
            public int Id { get; set; }
            public int AccountId { get; set; } // Contractor Account Id
            public string AccountName { get; set; } 
            public string MultipleProjectId { get; set; } // this project only effect basic 
            public int SingleProjectId { get; set; }  
            public string SingleProject { get; set; }  
            public string MultipleProject { get; set; }  
            public long BasicAmount { get; set; }
            public DateTime InsertionDate { get; set; } // system date
        public List<EffectivePercentProjectMappingModel> EffectivePercentProjectMappings { get; set; }

    }
    public class EffectivePercentProjectMappingModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

    }
}
