using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class DataAnalysis
    {
        public int Id { get; set; }

        [Required]
        public string AnalysisName { get; set; } = string.Empty;

        [Required]
        public string AnalysisType { get; set; } = string.Empty; // e.g., Statistical, Anomaly, Trend, Compliance

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExecutionDate { get; set; }

        [Required]
        public string Status { get; set; } = "Queued"; // Queued, InProgress, Completed, Failed

        public string DataSource { get; set; } = string.Empty;

        public int RecordsAnalyzed { get; set; }

        public int AnomaliesFound { get; set; }

        public decimal CompliancePercentage { get; set; }

        public TimeSpan? ExecutionDuration { get; set; }

        public string AnalysisResult { get; set; } = string.Empty;

        public string Findings { get; set; } = string.Empty;

        public string Recommendations { get; set; } = string.Empty;

        public DateTime? AnalysisStartDate { get; set; }

        public DateTime? AnalysisEndDate { get; set; }

        public string FilterCriteria { get; set; } = string.Empty;

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public string ExecutedBy { get; set; } = string.Empty;

        public string FailureReason { get; set; } = string.Empty;

        public int RetryCount { get; set; }
    }
}
