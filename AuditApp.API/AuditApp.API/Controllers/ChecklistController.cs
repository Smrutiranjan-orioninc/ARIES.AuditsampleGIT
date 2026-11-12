using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuditApp.API.Data;
using AuditApp.API.Models;

namespace AuditApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChecklistController : ControllerBase
    {
        private readonly AuditDbContext _context;

        public ChecklistController(AuditDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChecklistItem>>> GetChecklistItems()
        {
            return await _context.ChecklistItems.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChecklistItem>> GetChecklistItem(int id)
        {
            if (item.Id != 0) 
            {
                return BadRequest("New checklist item should not have an Id.");
            }
            var item = await _context.ChecklistItems.FindAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<ChecklistItem>> PostChecklistItem(ChecklistItem item)
        {
            if (item.Id != 0) 
            {
                return BadRequest("New checklist item should not have an Id.");
            }
            _context.ChecklistItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetChecklistItem", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutChecklistItem(int id, ChecklistItem item)
        {
            if (id != item.Id) 
            {
                return BadRequest();
            }
            var existing = await _context.ChecklistItems.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }
            _context.Entry(existing).CurrentValues.SetValues(item);
            _context.Entry(existing).State = EntityState.Detached;
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChecklistItem(int id)
        {
            var item = await _context.ChecklistItems.FindAsync(id);
            if (item == null) 
            {
                return NotFound();
            }
            _context.ChecklistItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
