using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ExpenseManagment.Data.Common
{
    class AuditEntry
    {
        public EntityEntry Entry { get; }
        public string TableName { get; set; }
        public string ActionType { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();

        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
            KeyValues = new Dictionary<string, object>();
            foreach (var property in entry.Properties)
            {
                KeyValues[property.Metadata.Name] = property.CurrentValue;
            }
        }

    }
}
