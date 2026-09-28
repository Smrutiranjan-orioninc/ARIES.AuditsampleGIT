using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class Settlement
    {
        public int Id { get; set; }

        [Required]
        public string SettlementName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [Required]
        public string Status { get; set; } = "Active"; // Active, Inactive, Pending

        public decimal Amount { get; set; }

        public string SettlementType { get; set; } = string.Empty; // e.g., Financial, Operational

        public string ResolutionReference { get; set; } = string.Empty;

        public DateTime? SettlementDate { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public string ModifiedBy { get; set; } = string.Empty;
    }
}
