using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;

namespace DotNetCoreSqlDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionSellingPnLTrackersController : ControllerBase
    {
        private readonly CoreDbContext _context;

        public OptionSellingPnLTrackersController(CoreDbContext context)
        {
            _context = context;
        }

        // GET: api/OptionSellingPnLTrackers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OptionSellingPnLTracker>>> GetOptionSellingPnLTracker()
        {
          if (_context.OptionSellingPnLTracker == null)
          {
              return NotFound();
          }
            return await _context.OptionSellingPnLTracker.ToListAsync();
        }

        // GET: api/OptionSellingPnLTrackers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OptionSellingPnLTracker>> GetOptionSellingPnLTracker(int id)
        {
          if (_context.OptionSellingPnLTracker == null)
          {
              return NotFound();
          }
            var optionSellingPnLTracker = await _context.OptionSellingPnLTracker.FindAsync(id);

            if (optionSellingPnLTracker == null)
            {
                return NotFound();
            }

            return optionSellingPnLTracker;
        }

        // PUT: api/OptionSellingPnLTrackers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOptionSellingPnLTracker(int id, OptionSellingPnLTracker optionSellingPnLTracker)
        {
            if (id != optionSellingPnLTracker.id)
            {
                return BadRequest();
            }

            _context.Entry(optionSellingPnLTracker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OptionSellingPnLTrackerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/OptionSellingPnLTrackers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OptionSellingPnLTracker>> PostOptionSellingPnLTracker(OptionSellingPnLTracker optionSellingPnLTracker)
        {
          if (_context.OptionSellingPnLTracker == null)
          {
              return Problem("Entity set 'CoreDbContext.OptionSellingPnLTracker'  is null.");
          }
            _context.OptionSellingPnLTracker.Add(optionSellingPnLTracker);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOptionSellingPnLTracker", new { id = optionSellingPnLTracker.id }, optionSellingPnLTracker);
        }

        // DELETE: api/OptionSellingPnLTrackers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOptionSellingPnLTracker(int id)
        {
            if (_context.OptionSellingPnLTracker == null)
            {
                return NotFound();
            }
            var optionSellingPnLTracker = await _context.OptionSellingPnLTracker.FindAsync(id);
            if (optionSellingPnLTracker == null)
            {
                return NotFound();
            }

            _context.OptionSellingPnLTracker.Remove(optionSellingPnLTracker);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OptionSellingPnLTrackerExists(int id)
        {
            return (_context.OptionSellingPnLTracker?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
