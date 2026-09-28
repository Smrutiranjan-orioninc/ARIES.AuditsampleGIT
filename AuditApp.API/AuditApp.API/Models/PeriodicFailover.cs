using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class PeriodicFailover
    {
        public int Id { get; set; }

        [Required]
        public string FailoverName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExecutionDate { get; set; }

        [Required]
        public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Success, Failed, Rollback

        public string SourceEnvironment { get; set; } = string.Empty;

        public string TargetEnvironment { get; set; } = string.Empty;

        public string FailoverType { get; set; } = string.Empty; // e.g., Database, Application, Infrastructure

        public DateTime? ScheduledTime { get; set; }

        public TimeSpan? ExecutionDuration { get; set; }

        public string FailureReason { get; set; } = string.Empty;

        public string RollbackStatus { get; set; } = string.Empty; // Pending, InProgress, Success, Failed

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public string ExecutedBy { get; set; } = string.Empty;

        public int RetryCount { get; set; }

        public DateTime? LastRetryDate { get; set; }
    }
}
