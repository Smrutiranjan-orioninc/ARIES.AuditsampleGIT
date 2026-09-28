using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuditApp.API.Data;
using AuditApp.API.Models;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace AuditApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataPersistencesController : ControllerBase
    {
        private readonly AuditDbContext _context;
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<DataPersistencesController> _logger;

        public DataPersistencesController(AuditDbContext context, TelemetryClient telemetryClient, ILogger<DataPersistencesController> logger)
        {
            _context = context;
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DataPersistence>>> GetDataPersistences()
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string>
            {
                { "Operation", "GetDataPersistences" },
                { "Module", "DataPersistence" },
                { "CloudRoleName", "DataPersistence" },
                { "Method", "GET" }
            };

            try
            {
                var records = await _context.DataPersistences.ToListAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["RecordCount"] = records.Count.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();

                _telemetryClient.TrackEvent("DataPersistence_GetAll", properties);
                _logger.LogInformation($"Retrieved {records.Count} data persistence records");

                return Ok(records);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();

                _telemetryClient.TrackEvent("DataPersistence_GetAll_Error", properties);
                _telemetryClient.TrackException(ex);
                _logger.LogError($"Error retrieving data persistences: {ex.Message}");

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DataPersistence>> GetDataPersistence(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string>
            {
                { "Operation", "GetDataPersistence" },
                { "Module", "DataPersistence" },
                { "CloudRoleName", "DataPersistence" },
                { "RecordId", id.ToString() },
                { "Method", "GET" }
            };

            try
            {
                var record = await _context.DataPersistences.FindAsync(id);
                stopwatch.Stop();

                if (record == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("DataPersistence_GetById_NotFound", properties);
                    return NotFound();
                }

                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["DataName"] = record.DataName;
                properties["Status"] = record.Status;
                properties["DataSize"] = record.DataSize.ToString();

                _telemetryClient.TrackEvent("DataPersistence_GetById", properties);
                _logger.LogInformation($"Retrieved data persistence record {id}");

                return Ok(record);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();

                _telemetryClient.TrackEvent("DataPersistence_GetById_Error", properties);
                _telemetryClient.TrackException(ex);
                _logger.LogError($"Error retrieving data persistence {id}: {ex.Message}");

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<DataPersistence>> PostDataPersistence(DataPersistence dataPersistence)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string>
            {
                { "Operation", "CreateDataPersistence" },
                { "Module", "DataPersistence" },
                { "CloudRoleName", "DataPersistence" },
                { "Method", "POST" },
                { "DataName", dataPersistence.DataName },
                { "DataType", dataPersistence.DataType },
                { "DataSize", dataPersistence.DataSize.ToString() }
            };

            try
            {
                dataPersistence.CreatedDate = DateTime.UtcNow;
                _context.DataPersistences.Add(dataPersistence);
                await _context.SaveChangesAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["RecordId"] = dataPersistence.Id.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["Status"] = dataPersistence.Status;

                _telemetryClient.TrackEvent("DataPersistence_Create", properties);
                _logger.LogInformation($"Data persistence record created: {dataPersistence.Id}");

                return CreatedAtAction("GetDataPersistence", new { id = dataPersistence.Id }, dataPersistence);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();

                _telemetryClient.TrackEvent("DataPersistence_Create_Error", properties);
                _telemetryClient.TrackException(ex);
                _logger.LogError($"Error creating data persistence: {ex.Message}");

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataPersistence(int id, DataPersistence dataPersistence)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string>
            {
                { "Operation", "UpdateDataPersistence" },
                { "Module", "DataPersistence" },
                { "CloudRoleName", "DataPersistence" },
                { "RecordId", id.ToString() },
                { "Method", "PUT" }
            };

            try
            {
                if (id != dataPersistence.Id)
                {
                    properties["OperationStatus"] = "BadRequest";
                    _telemetryClient.TrackEvent("DataPersistence_Update_IdMismatch", properties);
                    return BadRequest();
                }

                var existing = await _context.DataPersistences.FindAsync(id);
                if (existing == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("DataPersistence_Update_NotFound", properties);
                    return NotFound();
                }

                dataPersistence.ModifiedDate = DateTime.UtcNow;
                _context.Entry(existing).State = EntityState.Detached;
                _context.Entry(dataPersistence).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["Status"] = dataPersistence.Status;

                _telemetryClient.TrackEvent("DataPersistence_Update", properties);
                _logger.LogInformation($"Data persistence record updated: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();

                _telemetryClient.TrackEvent("DataPersistence_Update_Error", properties);
                _telemetryClient.TrackException(ex);
                _logger.LogError($"Error updating data persistence {id}: {ex.Message}");

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDataPersistence(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string>
            {
                { "Operation", "DeleteDataPersistence" },
                { "Module", "DataPersistence" },
                { "CloudRoleName", "DataPersistence" },
                { "RecordId", id.ToString() },
                { "Method", "DELETE" }
            };

            try
            {
                var dataPersistence = await _context.DataPersistences.FindAsync(id);
                if (dataPersistence == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("DataPersistence_Delete_NotFound", properties);
                    return NotFound();
                }

                _context.DataPersistences.Remove(dataPersistence);
                await _context.SaveChangesAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["DataName"] = dataPersistence.DataName;

                _telemetryClient.TrackEvent("DataPersistence_Delete", properties);
                _logger.LogInformation($"Data persistence record deleted: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();

                _telemetryClient.TrackEvent("DataPersistence_Delete_Error", properties);
                _telemetryClient.TrackException(ex);
                _logger.LogError($"Error deleting data persistence {id}: {ex.Message}");

                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
