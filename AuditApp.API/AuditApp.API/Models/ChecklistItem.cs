using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class ChecklistItem
    {
        public int Id { get; set; }
        public int AuditId { get; set; }
        public Audit? Audit { get; set; }
        [Required]
        public string TaskName { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
