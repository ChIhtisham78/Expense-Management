namespace ExpenseManagment.Data.Common
{
    public class ProjectTransactionModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public DateTime TransactionDate { get; set; }
        public long Amount { get; set; }
        public string Desc { get; set; }
    }
}
