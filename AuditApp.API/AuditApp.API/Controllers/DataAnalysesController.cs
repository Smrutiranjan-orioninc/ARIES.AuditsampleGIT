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
    public class DataAnalysesController : ControllerBase
    {
        private readonly AuditDbContext _context;
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<DataAnalysesController> _logger;

        public DataAnalysesController(AuditDbContext context, TelemetryClient telemetryClient, ILogger<DataAnalysesController> logger)
        {
            _context = context;
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DataAnalysis>>> GetDataAnalyses()
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "GetDataAnalyses" }, { "Module", "DataAnalysis" }, { "CloudRoleName", "DataAnalysis" }, { "Method", "GET" } };

            try
            {
                var records = await _context.DataAnalyses.ToListAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["RecordCount"] = records.Count.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataAnalysis_GetAll", properties);
                return Ok(records);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataAnalysis_GetAll_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DataAnalysis>> GetDataAnalysis(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "GetDataAnalysis" }, { "Module", "DataAnalysis" }, { "CloudRoleName", "DataAnalysis" }, { "AnalysisId", id.ToString() }, { "Method", "GET" } };

            try
            {
                var analysis = await _context.DataAnalyses.FindAsync(id);
                stopwatch.Stop();
                if (analysis == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("DataAnalysis_GetById_NotFound", properties);
                    return NotFound();
                }
                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["AnalysisName"] = analysis.AnalysisName;
                properties["Status"] = analysis.Status;
                _telemetryClient.TrackEvent("DataAnalysis_GetById", properties);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataAnalysis_GetById_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<DataAnalysis>> PostDataAnalysis(DataAnalysis analysis)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "CreateDataAnalysis" }, { "Module", "DataAnalysis" }, { "CloudRoleName", "DataAnalysis" }, { "Method", "POST" }, { "AnalysisName", analysis.AnalysisName }, { "AnalysisType", analysis.AnalysisType } };

            try
            {
                analysis.CreatedDate = DateTime.UtcNow;
                _context.DataAnalyses.Add(analysis);
                await _context.SaveChangesAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["AnalysisId"] = analysis.Id.ToString();
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataAnalysis_Create", properties);
                _telemetryClient.TrackTrace("Create new data analysis", SeverityLevel.Information, properties);
                _logger.LogInformation($"Data analysis started: {analysis.Id}");
                return CreatedAtAction("GetDataAnalysis", new { id = analysis.Id }, analysis);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataAnalysis_Create_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataAnalysis(int id, DataAnalysis analysis)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "UpdateDataAnalysis" }, { "Module", "DataAnalysis" }, { "CloudRoleName", "DataAnalysis" }, { "AnalysisId", id.ToString() }, { "Method", "PUT" } };

            try
            {
                if (id != analysis.Id) return BadRequest();
                if (!_context.DataAnalyses.Any(x => x.Id == id)) return NotFound();

                _context.Entry(analysis).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                stopwatch.Stop();

                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                properties["Status"] = analysis.Status;
                properties["AnomaliesFound"] = analysis.AnomaliesFound.ToString();
                _telemetryClient.TrackEvent("DataAnalysis_Update", properties);
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataAnalysis_Update_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDataAnalysis(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var properties = new Dictionary<string, string> { { "Operation", "DeleteDataAnalysis" }, { "Module", "DataAnalysis" }, { "CloudRoleName", "DataAnalysis" }, { "AnalysisId", id.ToString() }, { "Method", "DELETE" } };

            try
            {
                var analysis = await _context.DataAnalyses.FindAsync(id);
                if (analysis == null)
                {
                    properties["OperationStatus"] = "NotFound";
                    _telemetryClient.TrackEvent("DataAnalysis_Delete_NotFound", properties);
                    return NotFound();
                }
                _context.DataAnalyses.Remove(analysis);
                await _context.SaveChangesAsync();
                stopwatch.Stop();
                properties["OperationStatus"] = "Success";
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataAnalysis_Delete", properties);
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                properties["OperationStatus"] = "Failed";
                properties["ErrorMessage"] = ex.Message;
                properties["DurationMs"] = stopwatch.ElapsedMilliseconds.ToString();
                _telemetryClient.TrackEvent("DataAnalysis_Delete_Error", properties);
                _telemetryClient.TrackException(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
