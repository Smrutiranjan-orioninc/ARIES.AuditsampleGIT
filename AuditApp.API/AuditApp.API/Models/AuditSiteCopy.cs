using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class AuditSiteCopy
    {
        public int Id { get; set; }

        [Required]
        public string CopyName { get; set; } = string.Empty;

        public int SourceSiteId { get; set; }

        public int DestinationSiteId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExecutionDate { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Failed

        public long DataSize { get; set; } // in bytes

        public int TotalRecords { get; set; }

        public int CopiedRecords { get; set; }

        public int FailedRecords { get; set; }

        public TimeSpan? ExecutionDuration { get; set; }

        public string FailureReason { get; set; } = string.Empty;

        public string CopyType { get; set; } = string.Empty; // e.g., Full, Incremental, Delta

        public DateTime? LastSuccessfulCopyDate { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public string ExecutedBy { get; set; } = string.Empty;

        public bool IsVerified { get; set; }
    }
}
