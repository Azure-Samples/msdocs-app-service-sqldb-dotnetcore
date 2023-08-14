using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;

namespace DotNetCoreSqlDb.Controllers
{
    public class OrderBookDemoesController : Controller
    {
        private readonly CoreDbContext _context;

        public OrderBookDemoesController(CoreDbContext context)
        {
            _context = context;
        }

        // GET: OrderBookDemoes
        public async Task<IActionResult> Index()
        {
              return _context.OrderBookDemo != null ? 
                          View(await _context.OrderBookDemo.ToListAsync()) :
                          Problem("Entity set 'CoreDbContext.OrderBookDemo'  is null.");
        }

        // GET: OrderBookDemoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderBookDemo == null)
            {
                return NotFound();
            }

            var orderBookDemo = await _context.OrderBookDemo
                .FirstOrDefaultAsync(m => m.id == id);
            if (orderBookDemo == null)
            {
                return NotFound();
            }

            return View(orderBookDemo);
        }

        // GET: OrderBookDemoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderBookDemoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,symbol,optionType,optionDate,strikePrice,openDateTime,openBatch,openBatchDateTime,openSinalId,openStokePrice,openCost,closeDateTime,closeBatch,closeBatchDateTime,closeSinalId,closeStokePrice,closeCost,PnL")] OrderBookDemo orderBookDemo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderBookDemo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderBookDemo);
        }

        // GET: OrderBookDemoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderBookDemo == null)
            {
                return NotFound();
            }

            var orderBookDemo = await _context.OrderBookDemo.FindAsync(id);
            if (orderBookDemo == null)
            {
                return NotFound();
            }
            return View(orderBookDemo);
        }

        // POST: OrderBookDemoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,symbol,optionType,optionDate,strikePrice,openDateTime,openBatch,openBatchDateTime,openSinalId,openStokePrice,openCost,closeDateTime,closeBatch,closeBatchDateTime,closeSinalId,closeStokePrice,closeCost,PnL")] OrderBookDemo orderBookDemo)
        {
            if (id != orderBookDemo.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderBookDemo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderBookDemoExists(orderBookDemo.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orderBookDemo);
        }

        // GET: OrderBookDemoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderBookDemo == null)
            {
                return NotFound();
            }

            var orderBookDemo = await _context.OrderBookDemo
                .FirstOrDefaultAsync(m => m.id == id);
            if (orderBookDemo == null)
            {
                return NotFound();
            }

            return View(orderBookDemo);
        }

        // POST: OrderBookDemoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderBookDemo == null)
            {
                return Problem("Entity set 'CoreDbContext.OrderBookDemo'  is null.");
            }
            var orderBookDemo = await _context.OrderBookDemo.FindAsync(id);
            if (orderBookDemo != null)
            {
                _context.OrderBookDemo.Remove(orderBookDemo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderBookDemoExists(int id)
        {
          return (_context.OrderBookDemo?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
