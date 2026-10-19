using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuditApp.API.Data;
using AuditApp.API.Models;

namespace AuditApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly AuditDbContext _context;

        public ClientsController(AuditDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            // Include each client's audits so callers don't need a second
            // round-trip to see audit history per client.
            return await _context.Clients.Include(c => c.Audits).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.Include(c => c.Audits).FirstOrDefaultAsync(c => c.Id == id);
            if (client == null) return NotFound();
            return client;
        }

        [HttpGet("{id}/audit-summary")]
        public async Task<ActionResult<IEnumerable<object>>> GetClientAuditSummary(int id)
        {
            var audits = await _context.Audits.Where(a => a.ClientId == id).ToListAsync();
            var summary = audits.Select(a => new
            {
                a.Id,
                a.Title,
                a.Status,
                LeadAuditor = a.Auditor.FullName
            });
            return Ok(summary);
        }

        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetClient", new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.Id) return BadRequest();
            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
