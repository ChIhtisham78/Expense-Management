namespace ExpenseManagment.Data.Common
{
    public class AuditSettings
    {
        public bool AuditEnabled { get; set; }
        public string Tables { get; set; }
        public List<string> TableList
        {
            get
            {
                return this.Tables?
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToList() ?? new List<string>();
            }
        }
        public string SkipColumns { get; set; }
        public List<string> SkipColumnList
        {
            get
            {
                return this.SkipColumns?
                    .Split(",")
                    .Select(x => x.Trim())
                    .ToList() ?? new List<string>();
            }
        }
    }
}
