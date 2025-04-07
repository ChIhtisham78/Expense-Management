

namespace ExpenseManagment.Data.Common
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime InsertionDate { get; set; }
        public decimal ContractorEffectivePercent { get; set; }
    }
}
