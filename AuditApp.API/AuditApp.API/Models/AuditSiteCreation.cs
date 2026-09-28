using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class AuditSiteCreation
    {
        public int Id { get; set; }

        [Required]
        public string SiteName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [Required]
        public string Status { get; set; } = "Created"; // Created, Active, Inactive, Archived, Deleted

        public string SiteLocation { get; set; } = string.Empty;

        public string SiteCode { get; set; } = string.Empty;

        public string SiteType { get; set; } = string.Empty; // e.g., Primary, Secondary, Remote

        public int CapacityInGB { get; set; }

        public int UsedCapacityInGB { get; set; }

        public string SiteManager { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = string.Empty;

        public string ContactPhone { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public string ModifiedBy { get; set; } = string.Empty;

        public DateTime? ActivationDate { get; set; }
    }
}
