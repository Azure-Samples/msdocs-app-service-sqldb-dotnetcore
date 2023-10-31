using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authorization;

namespace DotNetCoreSqlDb.Controllers
{
    [ActionTimerFilter]
    [Authorize]
    public class ElectricalTestResultsController : Controller
    {
        private readonly MyDatabaseContext _context;
        private readonly IDistributedCache _cache;
        private readonly string _ElectricalTestResultsCacheKey = "ElectricalTestResultsList";

        public ElectricalTestResultsController(MyDatabaseContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: ElectricalTestResults
        public async Task<IActionResult> Index()
        {
            var results = new List<ElectricalTestResult>();
            byte[]? ResultsListByteArray;

            ResultsListByteArray = await _cache.GetAsync(_ElectricalTestResultsCacheKey);
            if (ResultsListByteArray != null && ResultsListByteArray.Length > 0)
            {
                results = ConvertData<ElectricalTestResult>.ByteArrayToObjectList(ResultsListByteArray);
            }
            else
            {
                results = await _context.ElectricalTestResults.ToListAsync();
                ResultsListByteArray = ConvertData<ElectricalTestResult>.ObjectListToByteArray(results);
                await _cache.SetAsync(_ElectricalTestResultsCacheKey, ResultsListByteArray);
            }

            return View(results);
        }

        // GET: ElectricalTestResults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await RetrieveResultFromCacheOrDb(id.Value);
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // GET: ElectricalTestResults/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ElectricalTestResults/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,JobNameOrNumber,CircuitLocation,CircuitNameOrDesignation,VisualInspection,ProtectionSizeOrType,NeutralNumber,NumberOfPhases,CableSize,EarthSize,ShortCircuitPass,InterconnectPass,PolarityPass,ContinuityOhms,InsulationResistance,FaultLoopImpedance,RcdTripTime,CreatedDate,TesterName,TimeStamp")] ElectricalTestResult result)
        {
            if (ModelState.IsValid)
            {
                _context.Add(result);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync(_ElectricalTestResultsCacheKey);
                return RedirectToAction(nameof(Index));
            }
            return View(result);
        }

        // GET: ElectricalTestResults/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await RetrieveResultFromCacheOrDb(id.Value);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

        // POST: ElectricalTestResults/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JobNameOrNumber,CircuitLocation,CircuitNameOrDesignation,VisualInspection,ProtectionSizeOrType,NeutralNumber,NumberOfPhases,CableSize,EarthSize,ShortCircuitPass,InterconnectPass,PolarityPass,ContinuityOhms,InsulationResistance,FaultLoopImpedance,RcdTripTime,CreatedDate,TesterName,TimeStamp")] ElectricalTestResult result)
        {
            if (id != result.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(result);
                    await _context.SaveChangesAsync();
                    await _cache.RemoveAsync(GetElectricalTestResultCacheKey(result.Id));
                    await _cache.RemoveAsync(_ElectricalTestResultsCacheKey);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ElectricalTestResultExists(result.Id))
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
            return View(result);
        }

        // GET: ElectricalTestResults/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await RetrieveResultFromCacheOrDb(id.Value);
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // POST: ElectricalTestResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _context.ElectricalTestResults.FindAsync(id);
            _context.ElectricalTestResults.Remove(result);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync(GetElectricalTestResultCacheKey(result.Id));
            await _cache.RemoveAsync(_ElectricalTestResultsCacheKey);
            return RedirectToAction(nameof(Index));
        }

        private bool ElectricalTestResultExists(int id)
        {
            return _context.ElectricalTestResults.Any(e => e.Id == id);
        }

        private string GetElectricalTestResultCacheKey(int? id)
        {
            return _ElectricalTestResultsCacheKey + "_&_" + id;
        }

        private async Task<ElectricalTestResult?> RetrieveResultFromCacheOrDb(int id)
        {
            byte[]? resultByteArray;
            ElectricalTestResult? result;

            resultByteArray = await _cache.GetAsync(GetElectricalTestResultCacheKey(id));

            if (resultByteArray != null && resultByteArray.Length > 0)
            {
                result = ConvertData<ElectricalTestResult>.ByteArrayToObject(resultByteArray);
            }
            else
            {
                result = await _context.ElectricalTestResults.FirstOrDefaultAsync(m => m.Id == id);
                if (result != null)
                {
                    resultByteArray = ConvertData<ElectricalTestResult>.ObjectToByteArray(result);
                    await _cache.SetAsync(GetElectricalTestResultCacheKey(id), resultByteArray);
                }
            }

            return result;
        }
    }
}