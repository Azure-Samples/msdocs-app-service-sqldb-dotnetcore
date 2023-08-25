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
    public class OrderBookOptionBuyingsController : Controller
    {
        private readonly CoreDbContext _context;

        public OrderBookOptionBuyingsController(CoreDbContext context)
        {
            _context = context;
        }

        // GET: OrderBookOptionBuyings
        public async Task<IActionResult> Index()
        {
              return _context.OrderBookOptionBuying != null ? 
                          View(await _context.OrderBookOptionBuying.ToListAsync()) :
                          Problem("Entity set 'CoreDbContext.OrderBookOptionBuying'  is null.");
        }

        // GET: OrderBookOptionBuyings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderBookOptionBuying == null)
            {
                return NotFound();
            }

            var orderBookOptionBuying = await _context.OrderBookOptionBuying
                .FirstOrDefaultAsync(m => m.id == id);
            if (orderBookOptionBuying == null)
            {
                return NotFound();
            }

            return View(orderBookOptionBuying);
        }

        // GET: OrderBookOptionBuyings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderBookOptionBuyings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,symbol,optionType,optionDate,strikePrice,openDateTime,openBatch,openBatchDateTime,openSinalId,openStokePrice,openCost,closeDateTime,closeBatch,closeBatchDateTime,closeSinalId,closeStokePrice,closeCost,PnL,PnLPercentage,percentFlag1,expectedPnL1,actualPnL1,batch1,percentFlag2,expectedPnL2,actualPnL2,batch2,percentFlag3,expectedPnL3,actualPnL3,batch3,percentFlag4,expectedPnL4,actualPnL4,batch4,percentFlag5,expectedPnL5,actualPnL5,batch5")] OrderBookOptionBuying orderBookOptionBuying)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderBookOptionBuying);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderBookOptionBuying);
        }

        // GET: OrderBookOptionBuyings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderBookOptionBuying == null)
            {
                return NotFound();
            }

            var orderBookOptionBuying = await _context.OrderBookOptionBuying.FindAsync(id);
            if (orderBookOptionBuying == null)
            {
                return NotFound();
            }
            return View(orderBookOptionBuying);
        }

        // POST: OrderBookOptionBuyings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,symbol,optionType,optionDate,strikePrice,openDateTime,openBatch,openBatchDateTime,openSinalId,openStokePrice,openCost,closeDateTime,closeBatch,closeBatchDateTime,closeSinalId,closeStokePrice,closeCost,PnL,PnLPercentage,percentFlag1,expectedPnL1,actualPnL1,batch1,percentFlag2,expectedPnL2,actualPnL2,batch2,percentFlag3,expectedPnL3,actualPnL3,batch3,percentFlag4,expectedPnL4,actualPnL4,batch4,percentFlag5,expectedPnL5,actualPnL5,batch5")] OrderBookOptionBuying orderBookOptionBuying)
        {
            if (id != orderBookOptionBuying.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderBookOptionBuying);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderBookOptionBuyingExists(orderBookOptionBuying.id))
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
            return View(orderBookOptionBuying);
        }

        // GET: OrderBookOptionBuyings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderBookOptionBuying == null)
            {
                return NotFound();
            }

            var orderBookOptionBuying = await _context.OrderBookOptionBuying
                .FirstOrDefaultAsync(m => m.id == id);
            if (orderBookOptionBuying == null)
            {
                return NotFound();
            }

            return View(orderBookOptionBuying);
        }

        // POST: OrderBookOptionBuyings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderBookOptionBuying == null)
            {
                return Problem("Entity set 'CoreDbContext.OrderBookOptionBuying'  is null.");
            }
            var orderBookOptionBuying = await _context.OrderBookOptionBuying.FindAsync(id);
            if (orderBookOptionBuying != null)
            {
                _context.OrderBookOptionBuying.Remove(orderBookOptionBuying);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderBookOptionBuyingExists(int id)
        {
          return (_context.OrderBookOptionBuying?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
