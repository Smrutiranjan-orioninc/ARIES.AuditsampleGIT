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
    public class RestoreAuditSitesController : ControllerBase
    {
        private readonly AuditDbContext _context;
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<RestoreAuditSitesController> _logger;

        public RestoreAuditSitesController(AuditDbContext context, TelemetryClient telemetryClient, ILogger<RestoreAuditSitesController> logger)
        {
            _context = context;
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestoreAuditSite>>> GetRestoreAuditSites()
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "GetRestoreAuditSites" }, { "Module", "RestoreAuditSite" }, { "CloudRoleName", "RestoreAuditSite" }, { "Method", "GET" } };

            try
            {
                var records = await _context.RestoreAuditSites.ToListAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["RecordCount"] = records.Count.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("RestoreAuditSite_GetAll", properties);
                return Ok(records);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("RestoreAuditSite_GetAll_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RestoreAuditSite>> GetRestoreAuditSite(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "GetRestoreAuditSite" }, { "Module", "RestoreAuditSite" }, { "CloudRoleName", "RestoreAuditSite" }, { "RestoreId", id.ToString() }, { "Method", "GET" } };

            try
            {
                var restore = await _context.RestoreAuditSites.FindAsync(id);
                stopwatch.Stop();
                if (restore == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("RestoreAuditSite_GetById_NotFound", properties);
                    return NotFound();
                }
                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["RestoreName"] = restore.RestoreName;
                properties["Status"] = restore.Status;
                _telemetryClient.TrackEvent("RestoreAuditSite_GetById", properties);
                return Ok(restore);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("RestoreAuditSite_GetById_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<RestoreAuditSite>> PostRestoreAuditSite(RestoreAuditSite restore)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "CreateRestoreAuditSite" }, { "Module", "RestoreAuditSite" }, { "CloudRoleName", "RestoreAuditSite" }, { "Method", "POST" }, { "RestoreName", restore.RestoreName }, { "RestoreType", restore.RestoreType }, { "TotalRecords", restore.TotalRecords.ToString() } };

            try
            {
                restore.CreatedDate = DateTime.UtcNow;
                _context.RestoreAuditSites.Add(restore);
                await _context.SaveChangesAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["RestoreId"] = restore.Id.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackTrace("Create new restore audit site", SeverityLevel.Information, properties);
                _telemetryClient.TrackEvent("RestoreAuditSite_Create", properties);
                _logger.LogInformation($"Audit site restore initiated: {restore.Id}");
                return CreatedAtAction("GetRestoreAuditSite", new { id = restore.Id }, restore);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("RestoreAuditSite_Create_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRestoreAuditSite(int id, RestoreAuditSite restore)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "UpdateRestoreAuditSite" }, { "Module", "RestoreAuditSite" }, { "CloudRoleName", "RestoreAuditSite" }, { "RestoreId", id.ToString() }, { "Method", "PUT" } };

            try
            {
                if (id != restore.Id) return BadRequest();
                if (!_context.RestoreAuditSites.Any(x => x.Id == id)) return NotFound();

                _context.Entry(restore).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["Status"] = restore.Status;
                properties["RestoredRecords"] = restore.RestoredRecords.ToString();
                _telemetryClient.TrackEvent("RestoreAuditSite_Update", properties);
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("RestoreAuditSite_Update_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestoreAuditSite(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "DeleteRestoreAuditSite" }, { "Module", "RestoreAuditSite" }, { "CloudRoleName", "RestoreAuditSite" }, { "RestoreId", id.ToString() }, { "Method", "DELETE" } };

            try
            {
                var restore = await _context.RestoreAuditSites.FindAsync(id);
                if (restore == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("RestoreAuditSite_Delete_NotFound", properties);
                    return NotFound();
                }
                _context.RestoreAuditSites.Remove(restore);
                await _context.SaveChangesAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("RestoreAuditSite_Delete", properties);
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("RestoreAuditSite_Delete_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
