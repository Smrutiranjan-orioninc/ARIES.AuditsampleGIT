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
    public class PeriodicFailoversController : ControllerBase
    {
        private readonly AuditDbContext _context;
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<PeriodicFailoversController> _logger;

        public PeriodicFailoversController(AuditDbContext context, TelemetryClient telemetryClient, ILogger<PeriodicFailoversController> logger)
        {
            _context = context;
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PeriodicFailover>>> GetPeriodicFailovers()
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "GetPeriodicFailovers" }, { "Module", "PeriodicFailover" }, { "CloudRoleName", "PeriodicFailover" }, { "Method", "GET" } };

            try
            {
                var records = await _context.PeriodicFailovers.ToListAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["RecordCount"] = records.Count.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("PeriodicFailover_GetAll", properties);
                _logger.LogInformation($"Retrieved {records.Count} periodic failover records");
                return Ok(records);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("PeriodicFailover_GetAll_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PeriodicFailover>> GetPeriodicFailover(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "GetPeriodicFailover" }, { "Module", "PeriodicFailover" }, { "CloudRoleName", "PeriodicFailover" }, { "RecordId", id.ToString() }, { "Method", "GET" } };

            try
            {
                var record = await _context.PeriodicFailovers.FindAsync(id);
                stopwatch.Stop();
                if (record == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("PeriodicFailover_GetById_NotFound", properties);
                    return NotFound();
                }
                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["FailoverName"] = record.FailoverName;
                properties["Status"] = record.Status;
                _telemetryClient.TrackEvent("PeriodicFailover_GetById", properties);
                return Ok(record);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("PeriodicFailover_GetById_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<PeriodicFailover>> PostPeriodicFailover(PeriodicFailover failover)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "CreatePeriodicFailover" }, { "Module", "PeriodicFailover" }, { "CloudRoleName", "PeriodicFailover" }, { "Method", "POST" }, { "FailoverName", failover.FailoverName }, { "FailoverType", failover.FailoverType } };

            try
            {
                failover.CreatedDate = DateTime.UtcNow;
                _context.PeriodicFailovers.Add(failover);
                await _context.SaveChangesAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["RecordId"] = failover.Id.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackTrace("Create new periodic failover", SeverityLevel.Information, properties);
                _telemetryClient.TrackEvent("PeriodicFailover_Create", properties);
                _logger.LogInformation($"Periodic failover created: {failover.Id}");
                return CreatedAtAction("GetPeriodicFailover", new { id = failover.Id }, failover);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("PeriodicFailover_Create_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPeriodicFailover(int id, PeriodicFailover failover)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "UpdatePeriodicFailover" }, { "Module", "PeriodicFailover" }, { "CloudRoleName", "PeriodicFailover" }, { "RecordId", id.ToString() }, { "Method", "PUT" } };

            try
            {
                if (id != failover.Id) return BadRequest();
                if (!_context.PeriodicFailovers.Any(x => x.Id == id)) return NotFound();

                failover.ExecutedBy = failover.ExecutedBy ?? string.Empty;
                _context.Entry(failover).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["Status"] = failover.Status;
                _telemetryClient.TrackEvent("PeriodicFailover_Update", properties);
                _logger.LogInformation($"Periodic failover updated: {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("PeriodicFailover_Update_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePeriodicFailover(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "DeletePeriodicFailover" }, { "Module", "PeriodicFailover" }, { "CloudRoleName", "PeriodicFailover" }, { "RecordId", id.ToString() }, { "Method", "DELETE" } };

            try
            {
                var failover = await _context.PeriodicFailovers.FindAsync(id);
                if (failover == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("PeriodicFailover_Delete_NotFound", properties);
                    return NotFound();
                }
                _context.PeriodicFailovers.Remove(failover);
                await _context.SaveChangesAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("PeriodicFailover_Delete", properties);
                _logger.LogInformation($"Periodic failover deleted: {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("PeriodicFailover_Delete_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
