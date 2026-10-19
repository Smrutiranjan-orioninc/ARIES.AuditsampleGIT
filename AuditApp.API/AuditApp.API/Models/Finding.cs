using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class Finding
    {
        public int Id { get; set; }
        public int AuditId { get; set; }
        public Audit? Audit { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = "Medium"; // e.g., Low, Medium, High, Critical
        public string Recommendation { get; set; } = string.Empty;
        public DateTime? ReviewedAt { get; set; }
    }
}
