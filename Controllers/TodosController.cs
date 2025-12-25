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
        private readonly string _TodoItemsCacheKey = "TodoItemsList";

        public TodosController(MyDatabaseContext context, IDistributedCache cache, ILogger<TodosController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        // GET: Todos
        // The cache logic is added with the help of GitHub Copilot
        public async Task<IActionResult> Index()
        {
            List<Todo> todoList;

            try
            {
                var cached = await _cache.GetAsync(_TodoItemsCacheKey);
                if (cached != null)
                {
                    _logger.LogInformation("Data from cache.");
                    todoList = JsonConvert.DeserializeObject<List<Todo>>(
                        Encoding.UTF8.GetString(cached)
                    )!;
                    return View(todoList);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB");
            }

            // Fallback → SQL
            _logger.LogInformation("Data from database.");
            todoList = await _context.Todo.ToListAsync();

            try
            {
                var serialized = JsonConvert.SerializeObject(todoList);
                await _cache.SetAsync(
                    _TodoItemsCacheKey,
                    Encoding.UTF8.GetBytes(serialized)
                );
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to write to Redis");
            }

            return View(todoList);
        }

        // GET: Todos/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            Todo? todoItem = null;

            try
            {
                var cached = await _cache.GetAsync(GetTodoItemCacheKey(id));
                if (cached != null)
                {
                    _logger.LogInformation("Data from cache.");
                    todoItem = JsonConvert.DeserializeObject<Todo>(
                        Encoding.UTF8.GetString(cached));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB");
            }

            if (todoItem == null)
            {
                _logger.LogInformation("Data from database.");
                todoItem = await _context.Todo.FirstOrDefaultAsync(m => m.ID == id);
            }

            if (todoItem == null)
                return NotFound();

            return View(todoItem);
        }


        // GET: Todos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Todos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Description,CreatedDate")] Todo todo)
        {
            if (!ModelState.IsValid)
                return View(todo);

            _context.Add(todo);
            await _context.SaveChangesAsync();

            try
            {
                await _cache.RemoveAsync(_TodoItemsCacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clear Redis cache");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Todos/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            Todo? todoItem = null;

            try
            {
                var cached = await _cache.GetAsync(GetTodoItemCacheKey(id));
                if (cached != null)
                {
                    _logger.LogInformation("Data from cache.");
                    todoItem = JsonConvert.DeserializeObject<Todo>(
                        Encoding.UTF8.GetString(cached));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB");
            }

            if (todoItem == null)
            {
                _logger.LogInformation("Data from database.");
                todoItem = await _context.Todo.FindAsync(id);
            }

            if (todoItem == null)
                return NotFound();

            return View(todoItem);
        }


        // POST: Todos/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Description,CreatedDate")] Todo todo)
        {
            if (id != todo.ID)
                return NotFound();

            if (!ModelState.IsValid)
                return View(todo);

            try
            {
                _context.Update(todo);
                await _context.SaveChangesAsync();

                try
                {
                    await _cache.RemoveAsync(GetTodoItemCacheKey(id));
                    await _cache.RemoveAsync(_TodoItemsCacheKey);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to clear Redis cache");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(todo.ID))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Todos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            Todo? todoItem = null;

            try
            {
                var cached = await _cache.GetAsync(GetTodoItemCacheKey(id));
                if (cached != null)
                {
                    _logger.LogInformation("Data from cache.");
                    todoItem = JsonConvert.DeserializeObject<Todo>(
                        Encoding.UTF8.GetString(cached));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB");
            }

            if (todoItem == null)
            {
                _logger.LogInformation("Data from database.");
                todoItem = await _context.Todo.FirstOrDefaultAsync(m => m.ID == id);
            }

            if (todoItem == null)
                return NotFound();

            return View(todoItem);
        }


        // POST: Todos/Delete/5
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

            try
            {
                await _cache.RemoveAsync(GetTodoItemCacheKey(id));
                await _cache.RemoveAsync(_TodoItemsCacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clear Redis cache");
            }

            return RedirectToAction(nameof(Index));
        }


        private bool TodoExists(int id)
        {
            return _context.Todo.Any(e => e.ID == id);
        }

        private string GetTodoItemCacheKey(int? id)
        {
            return $"{_TodoItemsCacheKey}_{id}";
        }
    }
}
