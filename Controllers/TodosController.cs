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
            var todoItems = await _cache.GetAsync(_TodoItemsCacheKey);
            if (todoItems != null)
            {
                _logger.LogInformation("Data from cache.");
                var todoList = JsonConvert.DeserializeObject<List<Todo>>(Encoding.UTF8.GetString(todoItems));
                return View(todoList);
            }
            else
            {
                _logger.LogInformation("Data from database.");
                var todoList = await _context.Todo.ToListAsync();
                var serializedTodoList = JsonConvert.SerializeObject(todoList);
                await _cache.SetAsync(_TodoItemsCacheKey, Encoding.UTF8.GetBytes(serializedTodoList));
                return View(todoList);
            }
        }

        // GET: Todos/Details/5
        // The cache logic is added with the help of GitHub Copilot
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _cache.GetAsync(GetTodoItemCacheKey(id));
            if (todo != null)
            {
                _logger.LogInformation("Data from cache.");
                var todoItem = JsonConvert.DeserializeObject<Todo>(Encoding.UTF8.GetString(todo));
                return View(todoItem);
            }
            else
            {
                _logger.LogInformation("Data from database.");
                var todoItem = await _context.Todo
                    .FirstOrDefaultAsync(m => m.ID == id);
                if (todoItem == null)
                {
                    return NotFound();
                }

                var serializedTodo = JsonConvert.SerializeObject(todoItem);
                await _cache.SetAsync(GetTodoItemCacheKey(id), Encoding.UTF8.GetBytes(serializedTodo));
                return View(todoItem);
            }
        }

        // GET: Todos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Todos/Create
        // The cache logic is added with the help of GitHub Copilot
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Description,CreatedDate")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(todo);
                await _context.SaveChangesAsync();

                // Clear the todo items cache
                await _cache.RemoveAsync(_TodoItemsCacheKey);

                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        // GET: Todos/Edit/5
        // The cache logic is added with the help of GitHub Copilot
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _cache.GetAsync(GetTodoItemCacheKey(id));
            if (todo != null)
            {
                _logger.LogInformation("Data from cache.");
                var todoItem = JsonConvert.DeserializeObject<Todo>(Encoding.UTF8.GetString(todo));
                return View(todoItem);
            }
            else
            {
                _logger.LogInformation("Data from database.");
                var todoItem = await _context.Todo.FindAsync(id);
                if (todoItem == null)
                {
                    return NotFound();
                }

                var serializedTodo = JsonConvert.SerializeObject(todoItem);
                await _cache.SetAsync(GetTodoItemCacheKey(id), Encoding.UTF8.GetBytes(serializedTodo));
                return View(todoItem);
            }
        }

        // POST: Todos/Edit/5
        // The cache logic is added with the help of GitHub Copilot
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Description,CreatedDate")] Todo todo)
        {
            if (id != todo.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todo);
                    await _context.SaveChangesAsync();

                    // Clear the todo item and todos list from the cache
                    await _cache.RemoveAsync(GetTodoItemCacheKey(id));
                    await _cache.RemoveAsync(_TodoItemsCacheKey);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoExists(todo.ID))
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
            return View(todo);
        }

        // GET: Todos/Delete/5
        // The cache logic is added with the help of GitHub Copilot
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _cache.GetAsync(GetTodoItemCacheKey(id));
            if (todo != null)
            {
                _logger.LogInformation("Data from cache.");
                var todoItem = JsonConvert.DeserializeObject<Todo>(Encoding.UTF8.GetString(todo));
                return View(todoItem);
            }
            else
            {
                _logger.LogInformation("Data from database.");
                var todoItem = await _context.Todo
                    .FirstOrDefaultAsync(m => m.ID == id);
                if (todoItem == null)
                {
                    return NotFound();
                }

                var serializedTodo = JsonConvert.SerializeObject(todoItem);
                await _cache.SetAsync(GetTodoItemCacheKey(id), Encoding.UTF8.GetBytes(serializedTodo));
                return View(todoItem);
            }
        }

        // POST: Todos/Delete/5
        // The cache logic is added with the help of GitHub Copilot
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todo = await _context.Todo.FindAsync(id);
            if (todo != null)
            {
                _context.Todo.Remove(todo);
            }

            await _context.SaveChangesAsync();

            // Clear the todo item and todos list from the cache
            await _cache.RemoveAsync(GetTodoItemCacheKey(id));
            await _cache.RemoveAsync(_TodoItemsCacheKey);

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
