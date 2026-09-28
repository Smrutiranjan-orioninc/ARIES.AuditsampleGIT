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
    public class DataSamplingsController : ControllerBase
    {
        private readonly AuditDbContext _context;
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<DataSamplingsController> _logger;

        public DataSamplingsController(AuditDbContext context, TelemetryClient telemetryClient, ILogger<DataSamplingsController> logger)
        {
            _context = context;
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DataSampling>>> GetDataSamplings()
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "GetDataSamplings" }, { "Module", "DataSampling" }, { "CloudRoleName", "DataSampling" }, { "Method", "GET" } };

            try
            {
                var records = await _context.DataSamplings.ToListAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["RecordCount"] = records.Count.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataSampling_GetAll", properties);
                return Ok(records);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataSampling_GetAll_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DataSampling>> GetDataSampling(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "GetDataSampling" }, { "Module", "DataSampling" }, { "CloudRoleName", "DataSampling" }, { "SamplingId", id.ToString() }, { "Method", "GET" } };

            try
            {
                var sampling = await _context.DataSamplings.FindAsync(id);
                stopwatch.Stop();
                if (sampling == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("DataSampling_GetById_NotFound", properties);
                    return NotFound();
                }
                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["SamplingName"] = sampling.SamplingName;
                properties["Status"] = sampling.Status;
                _telemetryClient.TrackEvent("DataSampling_GetById", properties);
                return Ok(sampling);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataSampling_GetById_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<DataSampling>> PostDataSampling(DataSampling sampling)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "CreateDataSampling" }, { "Module", "DataSampling" }, { "CloudRoleName", "DataSampling" }, { "Method", "POST" }, { "SamplingName", sampling.SamplingName }, { "SamplingMethod", sampling.SamplingMethod }, { "SampleSize", sampling.SampleSize.ToString() } };

            try
            {
                sampling.CreatedDate = DateTime.UtcNow;
                _context.DataSamplings.Add(sampling);
                await _context.SaveChangesAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["SamplingId"] = sampling.Id.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataSampling_Create", properties);
                _telemetryClient.TrackTrace("Create new data sampling", SeverityLevel.Information, properties);
                _logger.LogInformation($"Data sampling initiated: {sampling.Id}");
                return CreatedAtAction("GetDataSampling", new { id = sampling.Id }, sampling);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataSampling_Create_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataSampling(int id, DataSampling sampling)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "UpdateDataSampling" }, { "Module", "DataSampling" }, { "CloudRoleName", "DataSampling" }, { "SamplingId", id.ToString() }, { "Method", "PUT" } };

            try
            {
                if (id != sampling.Id) return BadRequest();
                if (!_context.DataSamplings.Any(x => x.Id == id)) return NotFound();

                _context.Entry(sampling).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["Status"] = sampling.Status;
                properties["SampledRecords"] = sampling.SampledRecords.ToString();
                _telemetryClient.TrackEvent("DataSampling_Update", properties);
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataSampling_Update_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDataSampling(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "DeleteDataSampling" }, { "Module", "DataSampling" }, { "CloudRoleName", "DataSampling" }, { "SamplingId", id.ToString() }, { "Method", "DELETE" } };

            try
            {
                var sampling = await _context.DataSamplings.FindAsync(id);
                if (sampling == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("DataSampling_Delete_NotFound", properties);
                    return NotFound();
                }
                _context.DataSamplings.Remove(sampling);
                await _context.SaveChangesAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataSampling_Delete", properties);
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataSampling_Delete_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
