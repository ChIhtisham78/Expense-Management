namespace ExpenseManagment.Data.Common
{
    public class ExpenceModel
    {
        public int Id { get; set; }
        public string ExpenceType { get; set; }
        public DateTime ExpenceDate { get; set; }
        public string ExpenceDesc { get; set; }
        public long ExpenceAmount { get; set; }
        public DateTime InsertionDate { get; set; }
        public int AccountId { get; set; }
    }
}
