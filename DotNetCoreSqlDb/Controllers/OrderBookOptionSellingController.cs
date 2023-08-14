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
    public class OrderBookOptionSellingController : ControllerBase
    {
        private readonly CoreDbContext _context;

        public OrderBookOptionSellingController(CoreDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderBookOptionSelling
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderBookOptionSelling>>> GetOrderBookOptionSelling()
        {
          if (_context.OrderBookOptionSelling == null)
          {
              return NotFound();
          }
            return await _context.OrderBookOptionSelling.ToListAsync();
        }

        // GET: api/OrderBookOptionSelling/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderBookOptionSelling>> GetOrderBookOptionSelling(int id)
        {
          if (_context.OrderBookOptionSelling == null)
          {
              return NotFound();
          }
            var orderBookOptionSelling = await _context.OrderBookOptionSelling.FindAsync(id);

            if (orderBookOptionSelling == null)
            {
                return NotFound();
            }

            return orderBookOptionSelling;
        }

        // PUT: api/OrderBookOptionSelling/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderBookOptionSelling(int id, OrderBookOptionSelling orderBookOptionSelling)
        {
            if (id != orderBookOptionSelling.id)
            {
                return BadRequest();
            }

            _context.Entry(orderBookOptionSelling).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderBookOptionSellingExists(id))
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

        // POST: api/OrderBookOptionSelling
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderBookOptionSelling>> PostOrderBookOptionSelling(OrderBookOptionSelling orderBookOptionSelling)
        {
          if (_context.OrderBookOptionSelling == null)
          {
              return Problem("Entity set 'CoreDbContext.OrderBookOptionSelling'  is null.");
          }
            _context.OrderBookOptionSelling.Add(orderBookOptionSelling);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderBookOptionSelling", new { id = orderBookOptionSelling.id }, orderBookOptionSelling);
        }

        // DELETE: api/OrderBookOptionSelling/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderBookOptionSelling(int id)
        {
            if (_context.OrderBookOptionSelling == null)
            {
                return NotFound();
            }
            var orderBookOptionSelling = await _context.OrderBookOptionSelling.FindAsync(id);
            if (orderBookOptionSelling == null)
            {
                return NotFound();
            }

            _context.OrderBookOptionSelling.Remove(orderBookOptionSelling);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderBookOptionSellingExists(int id)
        {
            return (_context.OrderBookOptionSelling?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
