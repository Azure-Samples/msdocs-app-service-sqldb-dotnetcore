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
    public class OptionBuyingPnLTrackersController : ControllerBase
    {
        private readonly CoreDbContext _context;

        public OptionBuyingPnLTrackersController(CoreDbContext context)
        {
            _context = context;
        }

        // GET: api/OptionBuyingPnLTrackers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OptionBuyingPnLTracker>>> GetOptionBuyingPnLTracker()
        {
          if (_context.OptionBuyingPnLTracker == null)
          {
              return NotFound();
          }
            return await _context.OptionBuyingPnLTracker.ToListAsync();
        }

        // GET: api/OptionBuyingPnLTrackers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OptionBuyingPnLTracker>> GetOptionBuyingPnLTracker(int id)
        {
          if (_context.OptionBuyingPnLTracker == null)
          {
              return NotFound();
          }
            var optionBuyingPnLTracker = await _context.OptionBuyingPnLTracker.FindAsync(id);

            if (optionBuyingPnLTracker == null)
            {
                return NotFound();
            }

            return optionBuyingPnLTracker;
        }

        // PUT: api/OptionBuyingPnLTrackers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOptionBuyingPnLTracker(int id, OptionBuyingPnLTracker optionBuyingPnLTracker)
        {
            if (id != optionBuyingPnLTracker.id)
            {
                return BadRequest();
            }

            _context.Entry(optionBuyingPnLTracker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OptionBuyingPnLTrackerExists(id))
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

        // POST: api/OptionBuyingPnLTrackers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OptionBuyingPnLTracker>> PostOptionBuyingPnLTracker(OptionBuyingPnLTracker optionBuyingPnLTracker)
        {
          if (_context.OptionBuyingPnLTracker == null)
          {
              return Problem("Entity set 'CoreDbContext.OptionBuyingPnLTracker'  is null.");
          }
            _context.OptionBuyingPnLTracker.Add(optionBuyingPnLTracker);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOptionBuyingPnLTracker", new { id = optionBuyingPnLTracker.id }, optionBuyingPnLTracker);
        }

        // DELETE: api/OptionBuyingPnLTrackers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOptionBuyingPnLTracker(int id)
        {
            if (_context.OptionBuyingPnLTracker == null)
            {
                return NotFound();
            }
            var optionBuyingPnLTracker = await _context.OptionBuyingPnLTracker.FindAsync(id);
            if (optionBuyingPnLTracker == null)
            {
                return NotFound();
            }

            _context.OptionBuyingPnLTracker.Remove(optionBuyingPnLTracker);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OptionBuyingPnLTrackerExists(int id)
        {
            return (_context.OptionBuyingPnLTracker?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
