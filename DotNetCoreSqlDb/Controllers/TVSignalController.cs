using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;
using DotNetCoreSqlDb.Service;
using Microsoft.CodeAnalysis.Options;
using StackExchange.Redis;

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

        // GET: api/TVSignal
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TVSignal>>> GetTVSignal()
        {
          if (_context.TVSignal == null)
          {    
                return NotFound();
          }
            return await _context.TVSignal.ToListAsync();
        }

        // GET: api/TVSignal/5
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

        // PUT: api/TVSignal/5
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

        // POST: api/TVSignal
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
            int signalId = tVSignal.id;

            if (tVSignal.Simbol.Equals("HLINE") && tVSignal.Source.Equals("LONG"))
            {

            }
            else if (tVSignal.Simbol.Equals("HLINE") && tVSignal.Source.Equals("SHORT"))
            {

            }


            if (tVSignal.Simbol.Equals("SPX") && (tVSignal.Source.Equals("EMA") || tVSignal.Simbol.Equals("HLINE")))
            {

                var optionDate = DateTime.Now.ToLocalTime().ToShortDateString();
                var hoursLeft = (Convert.ToDateTime(optionDate).ToLocalTime().Hour + 16) - DateTime.Now.ToLocalTime().Hour;

                string optionType = string.Empty;
                string closeOptionType = string.Empty;

                if (tVSignal.Signal.ToUpper().Equals("LONG") || tVSignal.Signal.ToUpper().Equals("SHORT"))
                {
                    DateTimeOffset now = (DateTimeOffset)DateTime.Now.ToLocalTime();
                    var batchId = now.ToUnixTimeSeconds();
                    var batchDateTime = now.ToString("yyyy-MM-dd hh:mm:ss.fff tt");

                    ETrade etrade = new ETrade();
                    var users = _context.User.ToList();

                    if (tVSignal.Signal.ToUpper().Equals("SHORT")) 
                    {
                        closeOptionType = "CALL";
                        optionType = "PUT";
                        var callOrderBook = _context.OrderBookDemo.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();

                        foreach (var order in callOrderBook)
                        {
                            var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
                            etrade.CloseOrder(_context, order, option, tVSignal.id);
                        }
                        if (tVSignal.Source.Equals("EMA") && tVSignal.Period.Equals("1"))
                        {
                            double awayFactor = (hoursLeft / 3) * 10;
                            double strikePriceNear = Math.Round(tVSignal.Price - awayFactor);
                            var putOrderBook = _context.OrderBookDemo.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == optionType && a.closeBatch <= 0).ToList();
                            if (putOrderBook.Count <= 0)
                            {
                                var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, Convert.ToInt64(strikePriceNear), optionType, batchId, batchDateTime);
                                etrade.OpenOrder(_context, option, tVSignal.id);
                            }
                        }
                    }

                    if (tVSignal.Signal.ToUpper().Equals("LONG"))
                    {
                        closeOptionType = "PUT";
                        optionType = "CALL";

                        var putOrderBook = _context.OrderBookDemo.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();
                        foreach (var order in putOrderBook)
                        {
                            var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
                            etrade.CloseOrder(_context, order, option, tVSignal.id);
                        }
                        if (tVSignal.Source.Equals("EMA") && tVSignal.Period.Equals("1"))
                        {
                            double awayFactor = (hoursLeft / 3) * 10;
                            double strikePriceNear = Math.Round(tVSignal.Price + awayFactor);
                            var callOrderBook = _context.OrderBookDemo.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == optionType && a.closeBatch <= 0).ToList();
                            if (callOrderBook.Count <= 0)
                            {
                                var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, Convert.ToInt64(strikePriceNear), optionType, batchId, batchDateTime);
                                etrade.OpenOrder(_context, option, tVSignal.id);
                            }
                        }
                    }
                }
            }

            return CreatedAtAction("GetTVSignal", new { id = tVSignal.id }, tVSignal);
        }

        // DELETE: api/TVSignal/5
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
