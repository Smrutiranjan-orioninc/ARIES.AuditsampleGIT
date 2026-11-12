using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuditApp.API.Data;
using AuditApp.API.Models;

namespace AuditApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditorsController : ControllerBase
    {
        private readonly AuditDbContext _context;

        public AuditorsController(AuditDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Auditor>>> GetAuditors()
        {
            // Include each auditor's assigned audits so callers can see
            // workload per auditor without a second round-trip.
            return await _context.Auditors.Include(a => a.Audits).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Auditor>> GetAuditor(int id)
        {
            var auditor = await _context.Auditors.Include(a => a.Audits).FirstOrDefaultAsync(a => a.Id == id);
            if (auditor == null) return NotFound();
            return auditor;
        }

        [HttpPost]
        public async Task<ActionResult<Auditor>> PostAuditor(Auditor auditor)
        {
            _context.Auditors.Add(auditor);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetAuditor", new { id = auditor.Id }, auditor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuditor(int id, Auditor auditor)
        {
            if (id != auditor.Id) return BadRequest();
            _context.Entry(auditor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuditor(int id)
        {
            var auditor = await _context.Auditors.FindAsync(id);
            if (auditor == null) return NotFound();
            _context.Auditors.Remove(auditor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
 	[HttpDelete("{date}")]
        public async Task<IActionResult> DeleteAuditordate(int id)
        {
            var auditor = await _context.Auditors.FindAsync(id);
            if (auditor == null) return NotFound();
            _context.Auditors.Remove(auditor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
