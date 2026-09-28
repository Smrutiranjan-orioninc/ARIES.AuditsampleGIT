using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class RestoreAuditSite
    {
        public int Id { get; set; }

        [Required]
        public string RestoreName { get; set; } = string.Empty;

        public int SourceSiteId { get; set; }

        public int TargetSiteId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExecutionDate { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Failed, Verified

        public DateTime? BackupDate { get; set; }

        public string BackupLocation { get; set; } = string.Empty;

        public long RestoredDataSize { get; set; } // in bytes

        public int TotalRecords { get; set; }

        public int RestoredRecords { get; set; }

        public int SkippedRecords { get; set; }

        public TimeSpan? ExecutionDuration { get; set; }

        public string FailureReason { get; set; } = string.Empty;

        public string RestoreType { get; set; } = string.Empty; // e.g., Full, Partial, Point-in-Time

        public DateTime? PointInTimeRestore { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public string ExecutedBy { get; set; } = string.Empty;

        public bool IsVerified { get; set; }

        public string VerificationResult { get; set; } = string.Empty;
    }
}
