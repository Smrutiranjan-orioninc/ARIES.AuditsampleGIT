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
    public class SettlementsController : ControllerBase
    {
        private readonly AuditDbContext _context;
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<SettlementsController> _logger;

        public SettlementsController(AuditDbContext context, TelemetryClient telemetryClient, ILogger<SettlementsController> logger)
        {
            _context = context;
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Settlement>>> GetSettlements()
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string>
            {
                { "Operation", "GetSettlements" },
                { "Module", "Settlement" },
                { "CloudRoleName", "Settlement" },
                { "Method", "GET" }
            };

            try
            {
                var settlements = await _context.Settlements.ToListAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["RecordCount"] = settlements.Count.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();

                _telemetryClient.TrackEvent("Settlement_GetAll", properties);
                _logger.LogInformation($"Successfully retrieved {settlements.Count} settlements in {stopwatch.ElapsedMilliseconds}ms");

                return Ok(settlements);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();

                _telemetryClient.TrackEvent("Settlement_GetAll_Error", properties);
                _telemetryClient.TrackException(ex);
                _logger.LogError($"Error retrieving settlements: {ex.Message}");

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Settlement>> GetSettlement(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string>
            {
                { "Operation", "GetSettlement" },
                { "Module", "Settlement" },
                { "CloudRoleName", "Settlement" },
                { "SettlementId", id.ToString() },
                { "Method", "GET" }
            };

            try
            {
                var settlement = await _context.Settlements.FindAsync(id);
                stopwatch.Stop();

                if (settlement == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("Settlement_GetById_NotFound", properties);
                    return NotFound();
                }

                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["SettlementName"] = settlement.SettlementName;
                properties["Status"] = settlement.Status;

                _telemetryClient.TrackEvent("Settlement_GetById", properties);
                _logger.LogInformation($"Retrieved settlement {id}: {settlement.SettlementName}");

                return Ok(settlement);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();

                _telemetryClient.TrackEvent("Settlement_GetById_Error", properties);
                _telemetryClient.TrackException(ex);
                _logger.LogError($"Error retrieving settlement {id}: {ex.Message}");

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Settlement>> PostSettlement(Settlement settlement)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string>
            {
                { "Operation", "CreateSettlement" },
                { "CloudRoleName", "Settlement" },
                { "Module", "Settlement" },
                { "Method", "POST" },
                { "SettlementName", settlement.SettlementName },
                { "Status", settlement.Status },
                { "Amount", settlement.Amount.ToString() }
            };

            try
            {
                settlement.CreatedDate = DateTime.UtcNow;
                _context.Settlements.Add(settlement);
                await _context.SaveChangesAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["SettlementId"] = settlement.Id.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["CreatedBy"] = settlement.CreatedBy;

                _telemetryClient.TrackEvent("Settlement_Create", properties);
                _logger.LogInformation($"Settlement created: {settlement.Id} - {settlement.SettlementName}");

                return CreatedAtAction("GetSettlement", new { id = settlement.Id }, settlement);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["ExceptionType"] = ex.GetType().Name;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();

                _telemetryClient.TrackEvent("Settlement_Create_Error", properties);
                _telemetryClient.TrackException(ex);
                _logger.LogError($"Error creating settlement: {ex.Message}");

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSettlement(int id, Settlement settlement)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string>
            {
                { "CloudRoleName", "Settlement" },
                { "Operation", "UpdateSettlement" },
                { "Module", "Settlement" },
                { "SettlementId", id.ToString() },
                { "Method", "PUT" },
                { "SettlementName", settlement.SettlementName }
            };

            try
            {
                if (id != settlement.Id)
                {
                    properties["OperationStatus"] = "BadRequest";
                    _telemetryClient.TrackEvent("Settlement_Update_IdMismatch", properties);
                    return BadRequest();
                }

                var existingSettlement = await _context.Settlements.FindAsync(id);
                if (existingSettlement == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("Settlement_Update_NotFound", properties);
                    return NotFound();
                }

                settlement.ModifiedDate = DateTime.UtcNow;
                _context.Entry(existingSettlement).State = EntityState.Detached;
                _context.Entry(settlement).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["Status"] = settlement.Status;
                properties["ModifiedBy"] = settlement.ModifiedBy;

                _telemetryClient.TrackEvent("Settlement_Update", properties);
                _logger.LogInformation($"Settlement updated: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();

                _telemetryClient.TrackEvent("Settlement_Update_Error", properties);
                _telemetryClient.TrackException(ex);
                _logger.LogError($"Error updating settlement {id}: {ex.Message}");

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSettlement(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string>
            {
                { "CloudRoleName", "Settlement" },
                { "Operation", "DeleteSettlement" },
                { "Module", "Settlement" },
                { "SettlementId", id.ToString() },
                { "Method", "DELETE" }
            };

            try
            {
                var settlement = await _context.Settlements.FindAsync(id);
                if (settlement == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("Settlement_Delete_NotFound", properties);
                    return NotFound();
                }

                _context.Settlements.Remove(settlement);
                await _context.SaveChangesAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["SettlementName"] = settlement.SettlementName;

                _telemetryClient.TrackEvent("Settlement_Delete", properties);
                _logger.LogInformation($"Settlement deleted: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();

                _telemetryClient.TrackEvent("Settlement_Delete_Error", properties);
                _telemetryClient.TrackException(ex);
                _logger.LogError($"Error deleting settlement {id}: {ex.Message}");

                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
