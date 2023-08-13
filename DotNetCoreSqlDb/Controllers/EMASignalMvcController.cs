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
    public class EMASignalMvcController : Controller
    {
        private readonly CoreDbContext _context;

        public EMASignalMvcController(CoreDbContext context)
        {
            _context = context;
        }

        // GET: EMASignalMvc
        public async Task<IActionResult> Index()
        {
              return _context.EMASignal != null ? 
                          View(await _context.EMASignal.ToListAsync()) :
                          Problem("Entity set 'CoreDbContext.EMASignal'  is null.");
        }

        // GET: EMASignalMvc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EMASignal == null)
            {
                return NotFound();
            }

            var eMASignal = await _context.EMASignal
                .FirstOrDefaultAsync(m => m.id == id);
            if (eMASignal == null)
            {
                return NotFound();
            }

            return View(eMASignal);
        }

        // GET: EMASignalMvc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EMASignalMvc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Simbol,OptionDate,Length,Signal,SignalDatetime,CreatedAt")] EMASignal eMASignal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eMASignal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eMASignal);
        }

        // GET: EMASignalMvc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EMASignal == null)
            {
                return NotFound();
            }

            var eMASignal = await _context.EMASignal.FindAsync(id);
            if (eMASignal == null)
            {
                return NotFound();
            }
            return View(eMASignal);
        }

        // POST: EMASignalMvc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Simbol,OptionDate,Length,Signal,SignalDatetime,CreatedAt")] EMASignal eMASignal)
        {
            if (id != eMASignal.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eMASignal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EMASignalExists(eMASignal.id))
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
            return View(eMASignal);
        }

        // GET: EMASignalMvc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EMASignal == null)
            {
                return NotFound();
            }

            var eMASignal = await _context.EMASignal
                .FirstOrDefaultAsync(m => m.id == id);
            if (eMASignal == null)
            {
                return NotFound();
            }

            return View(eMASignal);
        }

        // POST: EMASignalMvc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EMASignal == null)
            {
                return Problem("Entity set 'CoreDbContext.EMASignal'  is null.");
            }
            var eMASignal = await _context.EMASignal.FindAsync(id);
            if (eMASignal != null)
            {
                _context.EMASignal.Remove(eMASignal);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EMASignalExists(int id)
        {
          return (_context.EMASignal?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
