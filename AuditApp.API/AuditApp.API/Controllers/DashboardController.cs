using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuditApp.API.Data;

namespace AuditApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AuditDbContext _context;

        public DashboardController(AuditDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            var activeAudits = await _context.Audits.CountAsync(a => a.Status != "Completed");
            var clientsCount = await _context.Clients.CountAsync();
            var auditorsCount = await _context.Auditors.CountAsync();
            var findingsCount = await _context.Findings.CountAsync();
            var criticalFindings = await _context.Findings.CountAsync(f => f.Severity == "Critical");

            return Ok(new
            {
                ActiveAudits = activeAudits,
                ClientsCount = clientsCount,
                AuditorsCount = auditorsCount,
                FindingsCount = findingsCount,
                CriticalFindings = criticalFindings
            });
        }
    }
}
