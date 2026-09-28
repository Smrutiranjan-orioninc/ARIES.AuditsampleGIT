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
    public class AuditSiteCreationsController : ControllerBase
    {
        private readonly AuditDbContext _context;
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<AuditSiteCreationsController> _logger;

        public AuditSiteCreationsController(AuditDbContext context, TelemetryClient telemetryClient, ILogger<AuditSiteCreationsController> logger)
        {
            _context = context;
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditSiteCreation>>> GetAuditSiteCreations()
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "GetAuditSiteCreations" }, { "Module", "AuditSiteCreation" }, { "CloudRoleName", "AuditSiteCreation" }, { "Method", "GET" } };

            try
            {
                var records = await _context.AuditSiteCreations.ToListAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["RecordCount"] = records.Count.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCreation_GetAll", properties);
                _logger.LogInformation("Retrieved {Count} audit site creation records in {Duration}ms", records.Count, stopwatch.ElapsedMilliseconds);
                return Ok(records);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCreation_GetAll_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuditSiteCreation>> GetAuditSiteCreation(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "GetAuditSiteCreation" }, { "Module", "AuditSiteCreation" }, { "CloudRoleName", "AuditSiteCreation" }, { "SiteId", id.ToString() }, { "Method", "GET" } };

            try
            {
                var site = await _context.AuditSiteCreations.FindAsync(id);
                stopwatch.Stop();
                if (site == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("AuditSiteCreation_GetById_NotFound", properties);
                    _logger.LogWarning("Audit site creation {SiteId} was not found", id);
                    return NotFound();
                }
                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["SiteName"] = site.SiteName;
                _telemetryClient.TrackEvent("AuditSiteCreation_GetById", properties);
                _logger.LogInformation("Retrieved audit site creation {SiteId} ({SiteName}) in {Duration}ms", id, site.SiteName, stopwatch.ElapsedMilliseconds);
                return Ok(site);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCreation_GetById_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<AuditSiteCreation>> PostAuditSiteCreation(AuditSiteCreation site)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "CreateAuditSiteCreation" }, { "Module", "AuditSiteCreation" }, { "CloudRoleName", "AuditSiteCreation" }, { "Method", "POST" }, { "SiteName", site.SiteName }, { "SiteType", site.SiteType } };

            try
            {
                site.CreatedDate = DateTime.UtcNow;
                _context.AuditSiteCreations.Add(site);
                await _context.SaveChangesAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["SiteId"] = site.Id.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCreation_Create", properties);
                _telemetryClient.TrackTrace("Create new audit site creation", SeverityLevel.Information, properties);
                _logger.LogInformation($"Audit site created: {site.Id}");
                return CreatedAtAction("GetAuditSiteCreation", new { id = site.Id }, site);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCreation_Create_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuditSiteCreation(int id, AuditSiteCreation site)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "UpdateAuditSiteCreation" }, { "Module", "AuditSiteCreation" }, { "CloudRoleName", "AuditSiteCreation" }, { "SiteId", id.ToString() }, { "Method", "PUT" } };

            try
            {
                if (id != site.Id) return BadRequest();
                if (!_context.AuditSiteCreations.Any(x => x.Id == id)) return NotFound();

                site.ModifiedDate = DateTime.UtcNow;
                _context.Entry(site).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["Status"] = site.Status;
                _telemetryClient.TrackEvent("AuditSiteCreation_Update", properties);
                _logger.LogInformation("Audit site creation updated: {SiteId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCreation_Update_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuditSiteCreation(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "DeleteAuditSiteCreation" }, { "Module", "AuditSiteCreation" }, { "CloudRoleName", "AuditSiteCreation" }, { "SiteId", id.ToString() }, { "Method", "DELETE" } };

            try
            {
                var site = await _context.AuditSiteCreations.FindAsync(id);
                if (site == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("AuditSiteCreation_Delete_NotFound", properties);
                    _logger.LogWarning("Audit site creation {SiteId} was not found for delete", id);
                    return NotFound();
                }
                _context.AuditSiteCreations.Remove(site);
                await _context.SaveChangesAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCreation_Delete", properties);
                _logger.LogInformation("Audit site creation deleted: {SiteId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("AuditSiteCreation_Delete_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
