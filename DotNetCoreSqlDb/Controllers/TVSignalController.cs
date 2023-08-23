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

            tVSignal.CreatedAt = Help.GetEstDatetime();
            tVSignal.SignalDatetime = Help.GetEstDatetime(tVSignal.SignalDatetime);
            _context.TVSignal.Add(tVSignal);
            await _context.SaveChangesAsync();

            if (tVSignal.Simbol.Equals("SPX") || tVSignal.Simbol.Equals("SPY") && (tVSignal.Source.Equals("EMA") || tVSignal.Source.Equals("HLINE")))
            {
                var optionDate = Help.GetEstDatetime().ToShortDateString();
                var hoursLeft = 16 - Help.GetEstDatetime().Hour;
                string emaPeriodFactor = "3";

                ETrade eTrade = new ETrade();

                if (tVSignal.Source.Equals("HLINE") && tVSignal.Signal.ToUpper().Equals("CLOSEALL"))
                {
                    eTrade.CloseAll(_context);
                }

                if (tVSignal.Signal.ToUpper().Equals("LONG") || tVSignal.Signal.ToUpper().Equals("SHORT"))
                {
                    eTrade.RunOrderBookOptionBuying(_context, tVSignal, optionDate, hoursLeft, emaPeriodFactor);
                    eTrade.RunOrderBookOptionSelling(_context, tVSignal, optionDate, hoursLeft, emaPeriodFactor);
                }
            }
            return CreatedAtAction("GetTVSignal", new { id = tVSignal.id }, tVSignal);
        }

        //private void RunOrderBookOptionBuying(TVSignal tVSignal, string optionDate, int hoursLeft, string emaPeriodFactor)
        //{
        //    DateTimeOffset now = (DateTimeOffset)Help.GetEstDatetime();
        //    var batchId = now.ToUnixTimeSeconds();
        //    var batchDateTime = now.ToString("yyyy-MM-dd hh:mm:ss.fff tt");
        //    double awayFactor = (tVSignal.Price * 0.001) + (hoursLeft * 1.5);
        //    if (tVSignal.Simbol.ToUpper().Equals("SPY")) awayFactor = ((10 * tVSignal.Price * 0.001) + (hoursLeft * 1.5)) * 0.1;

        //    ETrade etrade = new ETrade();
        //    var users = _context.User.ToList();

        //    if (tVSignal.Signal.ToUpper().Equals("LONG") && tVSignal.Period.Equals(emaPeriodFactor))
        //    {
        //        string closeOptionType = "PUT";
        //        string optionType = "CALL";

        //        var putOrderBook = _context.OrderBookOptionBuying.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();
        //        foreach (var order in putOrderBook)
        //        {
        //            var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
        //            etrade.CloseOrderOptionBuying(_context, order, option, tVSignal.id);
        //        }
        //        double strikePriceNear = Math.Round(tVSignal.Price + awayFactor);
        //        var callOrderBook = _context.OrderBookOptionBuying.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == optionType && a.closeBatch <= 0).ToList();
        //        if (callOrderBook.Count <= 0)
        //        {
        //            var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, Convert.ToInt64(strikePriceNear), optionType, batchId, batchDateTime);
        //            etrade.OpenOrderOptionBuying(_context, option, tVSignal.id);
        //        }
        //    }
        //    else if (tVSignal.Signal.ToUpper().Equals("SHORT") && tVSignal.Period.Equals(emaPeriodFactor))
        //    {
        //        string closeOptionType = "CALL";
        //        string optionType = "PUT";
        //        var callOrderBook = _context.OrderBookOptionBuying.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();

        //        foreach (var order in callOrderBook)
        //        {
        //            var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
        //            etrade.CloseOrderOptionBuying(_context, order, option, tVSignal.id);
        //        }                  
        //        double strikePriceNear = Math.Round(tVSignal.Price - awayFactor);
        //        var putOrderBook = _context.OrderBookOptionBuying.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == optionType && a.closeBatch <= 0).ToList();
        //        if (putOrderBook.Count <= 0)
        //        {
        //            var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, Convert.ToInt64(strikePriceNear), optionType, batchId, batchDateTime);
        //            etrade.OpenOrderOptionBuying(_context, option, tVSignal.id);
        //        }
        //    }
        //}

        //private void RunOrderBookOptionSelling(TVSignal tVSignal, string optionDate, int hoursLeft, string emaPeriodFactor)
        //{
        //    DateTimeOffset now = (DateTimeOffset)Help.GetEstDatetime();
        //    var batchId = now.ToUnixTimeSeconds();
        //    var batchDateTime = now.ToString("yyyy-MM-dd hh:mm:ss.fff tt");
        //    double awayFactor = (tVSignal.Price * 0.00125) - hoursLeft;
        //    if (tVSignal.Simbol.ToUpper().Equals("SPY")) awayFactor = ((10 * tVSignal.Price * 0.00125) - hoursLeft) * 0.1;

        //    ETrade etrade = new ETrade();
        //    var users = _context.User.ToList();

        //    if (tVSignal.Signal.ToUpper().Equals("LONG") && tVSignal.Period.Equals(emaPeriodFactor))
        //    {
        //        string closeOptionType = "CALL";
        //        string optionType = "PUT";
        //        var callOrderBook = _context.OrderBookOptionSelling.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();

        //        foreach (var order in callOrderBook)
        //        {
        //            var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
        //            etrade.CloseOrderOptionSelling(_context, order, option, tVSignal.id);
        //        }

        //        double strikePriceNear = Math.Round(tVSignal.Price - awayFactor);
        //        var putOrderBook = _context.OrderBookOptionSelling.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == optionType && a.closeBatch <= 0).ToList();
        //        if (putOrderBook.Count <= 0)
        //        {
        //            var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, Convert.ToInt64(strikePriceNear), optionType, batchId, batchDateTime);
        //            etrade.OpenOrderOptionSelling(_context, option, tVSignal.id);
        //        }
        //    }

        //    if (tVSignal.Signal.ToUpper().Equals("SHORT") && tVSignal.Period.Equals(emaPeriodFactor))
        //    {
        //        string closeOptionType = "PUT";
        //        string optionType = "CALL";

        //        var putOrderBook = _context.OrderBookOptionSelling.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == closeOptionType && a.closeBatch <= 0).ToList();
        //        foreach (var order in putOrderBook)
        //        {
        //            var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, order.strikePrice, order.optionType, batchId, batchDateTime);
        //            etrade.CloseOrderOptionSelling(_context, order, option, tVSignal.id);
        //        }
        //        double strikePriceNear = Math.Round(tVSignal.Price + awayFactor);
        //        var callOrderBook = _context.OrderBookOptionSelling.ToList().Where(a => a.symbol.Contains(tVSignal.Simbol) && Convert.ToDateTime(a.optionDate) == Convert.ToDateTime(optionDate) && a.optionType == optionType && a.closeBatch <= 0).ToList();
        //        if (callOrderBook.Count <= 0)
        //        {
        //            var option = etrade.GetOptionDetails(_context, users[0], optionDate, tVSignal.Simbol, tVSignal.Price, Convert.ToInt64(strikePriceNear), optionType, batchId, batchDateTime);
        //            etrade.OpenOrderOptionSelling(_context, option, tVSignal.id);
        //        }
        //    }
        //}

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
