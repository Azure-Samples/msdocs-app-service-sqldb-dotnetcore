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
    public class OrderBookOptionBuyingController : ControllerBase
    {
        private readonly CoreDbContext _context;

        public OrderBookOptionBuyingController(CoreDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderBookOptionBuying
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderBookOptionBuying>>> GetOrderBookOptionBuying()
        {
          if (_context.OrderBookOptionBuying == null)
          {
              return NotFound();
          }
            return await _context.OrderBookOptionBuying.ToListAsync();
        }

        // GET: api/OrderBookOptionBuying/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderBookOptionBuying>> GetOrderBookOptionBuying(int id)
        {
          if (_context.OrderBookOptionBuying == null)
          {
              return NotFound();
          }
            var orderBookOptionBuying = await _context.OrderBookOptionBuying.FindAsync(id);

            if (orderBookOptionBuying == null)
            {
                return NotFound();
            }

            return orderBookOptionBuying;
        }

        // PUT: api/OrderBookOptionBuying/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderBookOptionBuying(int id, OrderBookOptionBuying orderBookOptionBuying)
        {
            if (id != orderBookOptionBuying.id)
            {
                return BadRequest();
            }

            _context.Entry(orderBookOptionBuying).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderBookOptionBuyingExists(id))
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

        // POST: api/OrderBookOptionBuying
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderBookOptionBuying>> PostOrderBookOptionBuying(OrderBookOptionBuying orderBookOptionBuying)
        {
          if (_context.OrderBookOptionBuying == null)
          {
              return Problem("Entity set 'CoreDbContext.OrderBookOptionBuying'  is null.");
          }
            _context.OrderBookOptionBuying.Add(orderBookOptionBuying);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderBookOptionBuying", new { id = orderBookOptionBuying.id }, orderBookOptionBuying);
        }

        // DELETE: api/OrderBookOptionBuying/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderBookOptionBuying(int id)
        {
            if (_context.OrderBookOptionBuying == null)
            {
                return NotFound();
            }
            var orderBookOptionBuying = await _context.OrderBookOptionBuying.FindAsync(id);
            if (orderBookOptionBuying == null)
            {
                return NotFound();
            }

            _context.OrderBookOptionBuying.Remove(orderBookOptionBuying);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderBookOptionBuyingExists(int id)
        {
            return (_context.OrderBookOptionBuying?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
