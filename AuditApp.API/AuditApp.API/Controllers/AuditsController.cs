using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuditApp.API.Data;
using AuditApp.API.Models;

namespace AuditApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditsController : ControllerBase
    {
        private readonly AuditDbContext _context;

        public AuditsController(AuditDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Audit>>> GetAudits()
        {
            return await _context.Audits.Include(a => a.Client).Include(a => a.Auditor).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Audit>> GetAudit(int id)
        {
            var audit = await _context.Audits.Include(a => a.Client).Include(a => a.Auditor).FirstOrDefaultAsync(a => a.Id == id);
            if (audit == null) return NotFound();
            return audit;
        }

        [HttpPost]
        public async Task<ActionResult<Audit>> PostAudit(Audit audit)
        {
            _context.Audits.Add(audit);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetAudit", new { id = audit.Id }, audit);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAudit(int id, Audit audit)
        {
            if (id != audit.Id) return BadRequest();
            _context.Entry(audit).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAudit(int id)
        {
            var audit = await _context.Audits.FindAsync(id);
            if (audit == null) return NotFound();
            _context.Audits.Remove(audit);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
