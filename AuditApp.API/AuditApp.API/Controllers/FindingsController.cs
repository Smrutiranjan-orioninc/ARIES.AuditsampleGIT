using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuditApp.API.Data;
using AuditApp.API.Models;

namespace AuditApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FindingsController : ControllerBase
    {
        private readonly AuditDbContext _context;

        public FindingsController(AuditDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Finding>>> GetFindings()
        {
            return await _context.Findings.Include(f => f.Audit).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Finding>> GetFinding(int id)
        {
            var finding = await _context.Findings.Include(f => f.Audit).FirstOrDefaultAsync(f => f.Id == id);
            if (finding == null) return NotFound();
            return finding;
        }

        [HttpPost]
        public async Task<ActionResult<Finding>> PostFinding(Finding finding)
        {
            _context.Findings.Add(finding);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetFinding", new { id = finding.Id }, finding);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFinding(int id, Finding finding)
        {
            if (id != finding.Id) return BadRequest();
            var existing = await _context.Findings.FindAsync(id);
            if (existing == null) return NotFound();
            _context.Entry(existing).State = EntityState.Detached;
            _context.Entry(finding).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFinding(int id)
        {
            var finding = await _context.Findings.FindAsync(id);
            if (finding == null) return NotFound();
            _context.Findings.Remove(finding);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
