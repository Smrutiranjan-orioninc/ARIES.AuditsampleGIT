using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class Audit
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public int AuditorId { get; set; }
        public Auditor? Auditor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "Planned"; // e.g., Planned, InProgress, Completed
    }
}
