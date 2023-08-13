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
    public class TVSignalViewController : Controller
    {
        private readonly CoreDbContext _context;

        public TVSignalViewController(CoreDbContext context)
        {
            _context = context;
        }

        // GET: TVSignalView
        public async Task<IActionResult> Index()
        {
              return _context.TVSignal != null ? 
                          View(await _context.TVSignal.ToListAsync()) :
                          Problem("Entity set 'CoreDbContext.TVSignal'  is null.");
        }

        // GET: TVSignalView/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TVSignal == null)
            {
                return NotFound();
            }

            var tVSignal = await _context.TVSignal
                .FirstOrDefaultAsync(m => m.id == id);
            if (tVSignal == null)
            {
                return NotFound();
            }

            return View(tVSignal);
        }

        // GET: TVSignalView/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TVSignalView/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Simbol,Volume,Period,Signal,Source,SignalDatetime")] TVSignal tVSignal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tVSignal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tVSignal);
        }

        // GET: TVSignalView/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TVSignal == null)
            {
                return NotFound();
            }

            var tVSignal = await _context.TVSignal.FindAsync(id);
            if (tVSignal == null)
            {
                return NotFound();
            }
            return View(tVSignal);
        }

        // POST: TVSignalView/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Simbol,Volume,Period,Signal,Source,SignalDatetime")] TVSignal tVSignal)
        {
            if (id != tVSignal.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tVSignal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TVSignalExists(tVSignal.id))
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
            return View(tVSignal);
        }

        // GET: TVSignalView/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TVSignal == null)
            {
                return NotFound();
            }

            var tVSignal = await _context.TVSignal
                .FirstOrDefaultAsync(m => m.id == id);
            if (tVSignal == null)
            {
                return NotFound();
            }

            return View(tVSignal);
        }

        // POST: TVSignalView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TVSignal == null)
            {
                return Problem("Entity set 'CoreDbContext.TVSignal'  is null.");
            }
            var tVSignal = await _context.TVSignal.FindAsync(id);
            if (tVSignal != null)
            {
                _context.TVSignal.Remove(tVSignal);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TVSignalExists(int id)
        {
          return (_context.TVSignal?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
