using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuditApp.API.Data;
using AuditApp.API.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System.Diagnostics;

namespace AuditApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditSiteCopiesController : ControllerBase
    {
        private readonly AuditDbContext _context;
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<AuditSiteCopiesController> _logger;

        public AuditSiteCopiesController(AuditDbContext context, TelemetryClient telemetryClient, ILogger<AuditSiteCopiesController> logger)
        {
            _context = context;
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditSiteCopy>>> GetAuditSiteCopies()
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "GetAuditSiteCopies" }, { "Module", "AuditSiteCopy" }, { "CloudRoleName", "AuditSiteCopy" }, { "Method", "GET" } };

            try
            {
                var records = await _context.AuditSiteCopies.ToListAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["RecordCount"] = records.Count.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCopy_GetAll", properties);
                _logger.LogInformation("Retrieved {Count} audit site copy records in {Duration}ms", records.Count, stopwatch.ElapsedMilliseconds);
                return Ok(records);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCopy_GetAll_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuditSiteCopy>> GetAuditSiteCopy(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "GetAuditSiteCopy" }, { "Module", "AuditSiteCopy" }, { "CloudRoleName", "AuditSiteCopy" }, { "CopyId", id.ToString() }, { "Method", "GET" } };

            try
            {
                var copy = await _context.AuditSiteCopies.FindAsync(id);
                stopwatch.Stop();
                if (copy == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("AuditSiteCopy_GetById_NotFound", properties);
                    _logger.LogWarning("Audit site copy {CopyId} was not found", id);
                    return NotFound();
                }
                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["CopyName"] = copy.CopyName;
                properties["Status"] = copy.Status;
                _telemetryClient.TrackEvent("AuditSiteCopy_GetById", properties);
                _logger.LogInformation("Retrieved audit site copy {CopyId} ({CopyName}) in {Duration}ms", id, copy.CopyName, stopwatch.ElapsedMilliseconds);
                return Ok(copy);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCopy_GetById_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<AuditSiteCopy>> PostAuditSiteCopy(AuditSiteCopy copy)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "CreateAuditSiteCopy" }, { "Module", "AuditSiteCopy" }, { "CloudRoleName", "AuditSiteCopy" }, { "Method", "POST" }, { "CopyName", copy.CopyName }, { "CopyType", copy.CopyType }, { "DataSize", copy.DataSize.ToString() } };

            try
            {
                copy.CreatedDate = DateTime.UtcNow;
                _context.AuditSiteCopies.Add(copy);
                await _context.SaveChangesAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["CopyId"] = copy.Id.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCopy_Create", properties);
                _telemetryClient.TrackTrace("Create new audit site copy", SeverityLevel.Information, properties);
                _logger.LogInformation($"Audit site copy created: {copy.Id}");
                return CreatedAtAction("GetAuditSiteCopy", new { id = copy.Id }, copy);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCopy_Create_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuditSiteCopy(int id, AuditSiteCopy copy)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "UpdateAuditSiteCopy" }, { "Module", "AuditSiteCopy" }, { "CloudRoleName", "AuditSiteCopy" }, { "CopyId", id.ToString() }, { "Method", "PUT" } };

            try
            {
                if (id != copy.Id) return BadRequest();
                if (!_context.AuditSiteCopies.Any(x => x.Id == id)) return NotFound();

                _context.Entry(copy).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["Status"] = copy.Status;
                properties["CopiedRecords"] = copy.CopiedRecords.ToString();
                _telemetryClient.TrackEvent("AuditSiteCopy_Update", properties);
                _logger.LogInformation("Audit site copy updated: {CopyId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCopy_Update_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuditSiteCopy(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "DeleteAuditSiteCopy" }, { "Module", "AuditSiteCopy" }, { "CloudRoleName", "AuditSiteCopy" }, { "CopyId", id.ToString() }, { "Method", "DELETE" } };

            try
            {
                var copy = await _context.AuditSiteCopies.FindAsync(id);
                if (copy == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("AuditSiteCopy_Delete_NotFound", properties);
                    _logger.LogWarning("Audit site copy {CopyId} was not found for delete", id);
                    return NotFound();
                }
                _context.AuditSiteCopies.Remove(copy);
                await _context.SaveChangesAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCopy_Delete", properties);
                _logger.LogInformation("Audit site copy deleted: {CopyId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCopy_Delete_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
