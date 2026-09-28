using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class Auditor
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // e.g., Senior Auditor, Lead Auditor
        public string Email { get; set; } = string.Empty;
    }
}
