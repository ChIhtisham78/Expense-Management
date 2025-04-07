using System.ComponentModel.DataAnnotations;

namespace ExpenseManagment.Data.Common
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } // Assuming you have user authentication
        public string UserEmail { get; set; }
        public string ActionType { get; set; } // e.g., INSERT, UPDATE, DELETE
        public string TableName { get; set; }
        public string KeyValues { get; set; } // Primary key value(s) of the affected entity
        public string OldValues { get; set; } // JSON string of the entity before the change
        public string NewValues { get; set; } // JSON string of the entity after the change
    }
}
