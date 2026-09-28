using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class DataSampling
    {
        public int Id { get; set; }

        [Required]
        public string SamplingName { get; set; } = string.Empty;

        [Required]
        public string SamplingMethod { get; set; } = string.Empty; // e.g., Random, Stratified, Systematic, Cluster

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExecutionDate { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Failed

        public string DataSource { get; set; } = string.Empty;

        public int TotalPopulation { get; set; }

        public int SampleSize { get; set; }

        public decimal SamplingRatePercentage { get; set; }

        public int SampledRecords { get; set; }

        public TimeSpan? ExecutionDuration { get; set; }

        public string SamplingCriteria { get; set; } = string.Empty;

        public string SampledDataLocation { get; set; } = string.Empty;

        public DateTime? SamplingStartDate { get; set; }

        public DateTime? SamplingEndDate { get; set; }

        public string ConfidenceLevel { get; set; } = string.Empty; // e.g., 95%, 99%

        public decimal MarginOfError { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public string ExecutedBy { get; set; } = string.Empty;

        public string FailureReason { get; set; } = string.Empty;

        public bool IsValidated { get; set; }

        public string ValidationResult { get; set; } = string.Empty;
    }
}
