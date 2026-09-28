using System.ComponentModel.DataAnnotations;

namespace AuditApp.API.Models
{
    public class DataPersistence
    {
        public int Id { get; set; }

        [Required]
        public string DataName { get; set; } = string.Empty;

        [Required]
        public string StorageLocation { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [Required]
        public string Status { get; set; } = "Stored"; // Stored, Archived, Deleted, In-Progress

        public long DataSize { get; set; } // in bytes

        public string DataType { get; set; } = string.Empty; // e.g., Audit Records, Financial Data

        public string EncryptionType { get; set; } = string.Empty; // e.g., AES-256, None

        public string BackupLocation { get; set; } = string.Empty;

        public DateTime? LastBackupDate { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public string ModifiedBy { get; set; } = string.Empty;

        public string RetentionPolicy { get; set; } = string.Empty; // e.g., 7 years
    }
}
