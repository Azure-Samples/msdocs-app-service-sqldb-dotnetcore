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
    public class EMASignalController : ControllerBase
    {
        private readonly CoreDbContext _context;

        public EMASignalController(CoreDbContext context)
        {
            _context = context;
        }

        // GET: api/EMASignals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EMASignal>>> GetEMASignal()
        {
          if (_context.EMASignal == null)
          {
              return NotFound();
          }
            return await _context.EMASignal.ToListAsync();
        }

        // GET: api/EMASignals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EMASignal>> GetEMASignal(int id)
        {
          if (_context.EMASignal == null)
          {
              return NotFound();
          }
            var eMASignal = await _context.EMASignal.FindAsync(id);

            if (eMASignal == null)
            {
                return NotFound();
            }

            return eMASignal;
        }

        // PUT: api/EMASignals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEMASignal(int id, EMASignal eMASignal)
        {
            if (id != eMASignal.id)
            {
                return BadRequest();
            }

            _context.Entry(eMASignal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EMASignalExists(id))
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

        // POST: api/EMASignals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EMASignal>> PostEMASignal(EMASignal eMASignal)
        {
          if (_context.EMASignal == null)
          {
              return Problem("Entity set 'CoreDbContext.EMASignal'  is null.");
          }
            _context.EMASignal.Add(eMASignal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEMASignal", new { id = eMASignal.id }, eMASignal);
        }

        // DELETE: api/EMASignals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEMASignal(int id)
        {
            if (_context.EMASignal == null)
            {
                return NotFound();
            }
            var eMASignal = await _context.EMASignal.FindAsync(id);
            if (eMASignal == null)
            {
                return NotFound();
            }

            _context.EMASignal.Remove(eMASignal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EMASignalExists(int id)
        {
            return (_context.EMASignal?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
