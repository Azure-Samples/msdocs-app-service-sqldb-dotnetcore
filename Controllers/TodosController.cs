using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace DotNetCoreSqlDb.Controllers
{
    [ActionTimerFilter]
    public class TodosController : Controller
    {
        private readonly ILogger<TodosController> _logger;
        private readonly MyDatabaseContext _context;
        private readonly IDistributedCache _cache;

        private const string TodoListCacheKey = "TodoItemsList";

        public TodosController(
            MyDatabaseContext context,
            IDistributedCache cache,
            ILogger<TodosController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        // =========================
        // GET: Todos
        // =========================
        public async Task<IActionResult> Index()
        {
            List<Todo>? todos = null;

            try
            {
                _logger.LogInformation("Index: trying Redis");
                var cached = await _cache.GetAsync(TodoListCacheKey);

                if (cached != null)
                {
                    _logger.LogInformation("Index: data from cache");
                    todos = JsonConvert.DeserializeObject<List<Todo>>(
                        Encoding.UTF8.GetString(cached));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Index: Redis unavailable");
            }

            if (todos == null)
            {
                _logger.LogInformation("Index: data from database");
                todos = await _context.Todo.ToListAsync();

                try
                {
                    await _cache.SetAsync(
                        TodoListCacheKey,
                        Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(todos))
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Index: failed to write Redis");
                }
            }

            return View(todos);
        }

        // =========================
        // GET: Todos/Details/5
        // =========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            Todo? todo = null;

            try
            {
                _logger.LogInformation("Details: trying Redis");
                var cached = await _cache.GetAsync(GetTodoCacheKey(id.Value));

                if (cached != null)
                {
                    _logger.LogInformation("Details: data from cache");
                    todo = JsonConvert.DeserializeObject<Todo>(
                        Encoding.UTF8.GetString(cached));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Details: Redis unavailable");
            }

            if (todo == null)
            {
                _logger.LogInformation("Details: data from database");
                todo = await _context.Todo.FirstOrDefaultAsync(t => t.ID == id);
            }

            return todo == null ? NotFound() : View(todo);
        }

        // =========================
        // GET: Todos/Create
        // =========================
        public IActionResult Create() => View();

        // =========================
        // POST: Todos/Create
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Todo todo)
        {
            if (!ModelState.IsValid) return View(todo);

            _context.Add(todo);
            await _context.SaveChangesAsync();

            await ClearCache();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // GET: Todos/Edit/5
        // =========================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            Todo? todo = null;

            try
            {
                _logger.LogInformation("Edit GET: trying Redis");
                var cached = await _cache.GetAsync(GetTodoCacheKey(id.Value));

                if (cached != null)
                {
                    _logger.LogInformation("Edit GET: data from cache");
                    todo = JsonConvert.DeserializeObject<Todo>(
                        Encoding.UTF8.GetString(cached));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Edit GET: Redis unavailable");
            }

            if (todo == null)
            {
                _logger.LogInformation("Edit GET: data from database");
                todo = await _context.Todo.FindAsync(id);
            }

            return todo == null ? NotFound() : View(todo);
        }

        // =========================
        // POST: Todos/Edit/5
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Todo todo)
        {
            if (id != todo.ID) return NotFound();
            if (!ModelState.IsValid) return View(todo);

            try
            {
                _context.Update(todo);
                await _context.SaveChangesAsync();
                await ClearCache();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(todo.ID)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // GET: Todos/Delete/5
        // =========================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var todo = await _context.Todo.FirstOrDefaultAsync(t => t.ID == id);
            return todo == null ? NotFound() : View(todo);
        }

        // =========================
        // POST: Todos/Delete/5
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todo = await _context.Todo.FindAsync(id);

            if (todo != null)
            {
                _context.Todo.Remove(todo);
                await _context.SaveChangesAsync();
            }

            await ClearCache();
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Helpers
        // =========================
        private async Task ClearCache()
        {
            try
            {
                await _cache.RemoveAsync(TodoListCacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clear Redis cache");
            }
        }

        private bool TodoExists(int id) =>
            _context.Todo.Any(e => e.ID == id);

        private static string GetTodoCacheKey(int id) =>
            $"TodoItem_{id}";
    }
}
