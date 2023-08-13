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
    public class TVSignalController : ControllerBase
    {
        private readonly CoreDbContext _context;

        public TVSignalController(CoreDbContext context)
        {
            _context = context;
        }

        // GET: api/TVSignals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TVSignal>>> GetTVSignal()
        {
          if (_context.TVSignal == null)
          {
              return NotFound();
          }
            return await _context.TVSignal.ToListAsync();
        }

        // GET: api/TVSignals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TVSignal>> GetTVSignal(int id)
        {
          if (_context.TVSignal == null)
          {
              return NotFound();
          }
            var tVSignal = await _context.TVSignal.FindAsync(id);

            if (tVSignal == null)
            {
                return NotFound();
            }

            return tVSignal;
        }

        // PUT: api/TVSignals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTVSignal(int id, TVSignal tVSignal)
        {
            if (id != tVSignal.id)
            {
                return BadRequest();
            }

            _context.Entry(tVSignal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TVSignalExists(id))
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

        // POST: api/TVSignals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TVSignal>> PostTVSignal(TVSignal tVSignal)
        {
          if (_context.TVSignal == null)
          {
              return Problem("Entity set 'CoreDbContext.TVSignal'  is null.");
          }
            _context.TVSignal.Add(tVSignal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTVSignal", new { id = tVSignal.id }, tVSignal);
        }

        // DELETE: api/TVSignals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTVSignal(int id)
        {
            if (_context.TVSignal == null)
            {
                return NotFound();
            }
            var tVSignal = await _context.TVSignal.FindAsync(id);
            if (tVSignal == null)
            {
                return NotFound();
            }

            _context.TVSignal.Remove(tVSignal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TVSignalExists(int id)
        {
            return (_context.TVSignal?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
