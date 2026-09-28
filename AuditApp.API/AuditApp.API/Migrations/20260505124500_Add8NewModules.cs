using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditApp.API.Migrations
{
    /// <inheritdoc />
    public partial class Add8NewModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditSiteCreations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CapacityInGB = table.Column<int>(type: "int", nullable: false),
                    UsedCapacityInGB = table.Column<int>(type: "int", nullable: false),
                    SiteManager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditSiteCreations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditSiteCopies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CopyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceSiteId = table.Column<int>(type: "int", nullable: false),
                    DestinationSiteId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CopyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalRecords = table.Column<int>(type: "int", nullable: false),
                    CopiedRecords = table.Column<int>(type: "int", nullable: false),
                    FailedRecords = table.Column<int>(type: "int", nullable: false),
                    DataSize = table.Column<long>(type: "bigint", nullable: false),
                    ExecutionDuration = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    LastSuccessfulCopyDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditSiteCopies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataAnalyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnalysisName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnalysisType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordsAnalyzed = table.Column<int>(type: "int", nullable: false),
                    AnomaliesFound = table.Column<int>(type: "int", nullable: false),
                    CompliancePercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ExecutionDuration = table.Column<TimeSpan>(type: "time", nullable: true),
                    AnalysisResult = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recommendations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnalysisStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnalysisEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FilterCriteria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataAnalyses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataPersistences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StorageLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataSize = table.Column<long>(type: "bigint", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackupLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastBackupDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RetentionPolicy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataPersistences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataSamplings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SamplingName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SamplingMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalPopulation = table.Column<int>(type: "int", nullable: false),
                    SampleSize = table.Column<int>(type: "int", nullable: false),
                    SamplingRatePercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SampledRecords = table.Column<int>(type: "int", nullable: false),
                    ExecutionDuration = table.Column<TimeSpan>(type: "time", nullable: true),
                    SamplingCriteria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SampledDataLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SamplingStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SamplingEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfidenceLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarginOfError = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsValidated = table.Column<bool>(type: "bit", nullable: false),
                    ValidationResult = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSamplings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PeriodicFailovers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FailoverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceEnvironment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetEnvironment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FailoverType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduledTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutionDuration = table.Column<TimeSpan>(type: "time", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RollbackStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    LastRetryDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodicFailovers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RestoreAuditSites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestoreName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceSiteId = table.Column<int>(type: "int", nullable: false),
                    TargetSiteId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackupDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BackupLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RestoredDataSize = table.Column<long>(type: "bigint", nullable: false),
                    TotalRecords = table.Column<int>(type: "int", nullable: false),
                    RestoredRecords = table.Column<int>(type: "int", nullable: false),
                    SkippedRecords = table.Column<int>(type: "int", nullable: false),
                    ExecutionDuration = table.Column<TimeSpan>(type: "time", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RestoreType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PointInTimeRestore = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerificationResult = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestoreAuditSites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settlements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettlementName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SettlementType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResolutionReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SettlementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settlements", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditSiteCreations_Status",
                table: "AuditSiteCreations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AuditSiteCopies_Status",
                table: "AuditSiteCopies",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DataAnalyses_Status",
                table: "DataAnalyses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DataPersistences_Status",
                table: "DataPersistences",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DataSamplings_Status",
                table: "DataSamplings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodicFailovers_Status",
                table: "PeriodicFailovers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RestoreAuditSites_Status",
                table: "RestoreAuditSites",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_Status",
                table: "Settlements",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditSiteCreations");

            migrationBuilder.DropTable(
                name: "AuditSiteCopies");

            migrationBuilder.DropTable(
                name: "DataAnalyses");

            migrationBuilder.DropTable(
                name: "DataPersistences");

            migrationBuilder.DropTable(
                name: "DataSamplings");

            migrationBuilder.DropTable(
                name: "PeriodicFailovers");

            migrationBuilder.DropTable(
                name: "RestoreAuditSites");

            migrationBuilder.DropTable(
                name: "Settlements");
        }
    }
}
