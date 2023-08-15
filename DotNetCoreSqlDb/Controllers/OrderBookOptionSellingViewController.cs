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
    public class OrderBookOptionSellingViewController : Controller
    {
        private readonly CoreDbContext _context;

        public OrderBookOptionSellingViewController(CoreDbContext context)
        {
            _context = context;
        }

        // GET: OrderBookOptionSellingView
        public async Task<IActionResult> Index()
        {
              return _context.OrderBookOptionSelling != null ? 
                          View(await _context.OrderBookOptionSelling.ToListAsync()) :
                          Problem("Entity set 'CoreDbContext.OrderBookOptionSelling'  is null.");
        }

        // GET: OrderBookOptionSellingView/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderBookOptionSelling == null)
            {
                return NotFound();
            }

            var orderBookOptionSelling = await _context.OrderBookOptionSelling
                .FirstOrDefaultAsync(m => m.id == id);
            if (orderBookOptionSelling == null)
            {
                return NotFound();
            }

            return View(orderBookOptionSelling);
        }

        // GET: OrderBookOptionSellingView/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderBookOptionSellingView/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,symbol,optionType,optionDate,strikePrice,openDateTime,openBatch,openBatchDateTime,openSinalId,openStokePrice,openCost,closeDateTime,closeBatch,closeBatchDateTime,closeSinalId,closeStokePrice,closeCost,PnL")] OrderBookOptionSelling orderBookOptionSelling)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderBookOptionSelling);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderBookOptionSelling);
        }

        // GET: OrderBookOptionSellingView/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderBookOptionSelling == null)
            {
                return NotFound();
            }

            var orderBookOptionSelling = await _context.OrderBookOptionSelling.FindAsync(id);
            if (orderBookOptionSelling == null)
            {
                return NotFound();
            }
            return View(orderBookOptionSelling);
        }

        // POST: OrderBookOptionSellingView/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,symbol,optionType,optionDate,strikePrice,openDateTime,openBatch,openBatchDateTime,openSinalId,openStokePrice,openCost,closeDateTime,closeBatch,closeBatchDateTime,closeSinalId,closeStokePrice,closeCost,PnL")] OrderBookOptionSelling orderBookOptionSelling)
        {
            if (id != orderBookOptionSelling.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderBookOptionSelling);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderBookOptionSellingExists(orderBookOptionSelling.id))
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
            return View(orderBookOptionSelling);
        }

        // GET: OrderBookOptionSellingView/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderBookOptionSelling == null)
            {
                return NotFound();
            }

            var orderBookOptionSelling = await _context.OrderBookOptionSelling
                .FirstOrDefaultAsync(m => m.id == id);
            if (orderBookOptionSelling == null)
            {
                return NotFound();
            }

            return View(orderBookOptionSelling);
        }

        // POST: OrderBookOptionSellingView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderBookOptionSelling == null)
            {
                return Problem("Entity set 'CoreDbContext.OrderBookOptionSelling'  is null.");
            }
            var orderBookOptionSelling = await _context.OrderBookOptionSelling.FindAsync(id);
            if (orderBookOptionSelling != null)
            {
                _context.OrderBookOptionSelling.Remove(orderBookOptionSelling);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderBookOptionSellingExists(int id)
        {
          return (_context.OrderBookOptionSelling?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
