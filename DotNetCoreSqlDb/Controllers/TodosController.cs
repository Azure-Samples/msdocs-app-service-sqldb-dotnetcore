using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace DotNetCoreSqlDb.Controllers
{
    [ActionTimerFilter]
    public class TodosController : Controller
    {
        private readonly MyDatabaseContext _context;
        private readonly IDistributedCache _cache;
        private readonly string _TodoItemsCacheKey = "TodoItemsList";

        public TodosController(MyDatabaseContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: Todos
        public async Task<IActionResult> Index()
        {
            var todos = new List<Todo>();
            byte[]? TodoListByteArray;

            TodoListByteArray = await _cache.GetAsync(_TodoItemsCacheKey);
            if (TodoListByteArray != null && TodoListByteArray.Length > 0)
            { 
                todos = ConvertData<Todo>.ByteArrayToObjectList(TodoListByteArray);
            }
            else 
            {
                todos = await _context.Todo.ToListAsync();
                TodoListByteArray = ConvertData<Todo>.ObjectListToByteArray(todos);
                await _cache.SetAsync(_TodoItemsCacheKey, TodoListByteArray);
            }

            return View(todos);
        }

        // GET: Todos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            byte[]? todoItemByteArray;
            Todo? todo;

            if (id == null)
            {
                return NotFound();
            }

            todoItemByteArray = await _cache.GetAsync(GetTodoItemCacheKey(id));

            if (todoItemByteArray != null && todoItemByteArray.Length > 0)
            {
                todo = ConvertData<Todo>.ByteArrayToObject(todoItemByteArray);
            }
            else 
            {
                todo = await _context.Todo
                .FirstOrDefaultAsync(m => m.ID == id);
            if (todo == null)
            {
                return NotFound();
            }

                todoItemByteArray = ConvertData<Todo>.ObjectToByteArray(todo);
                await _cache.SetAsync(GetTodoItemCacheKey(id), todoItemByteArray);
            }

            

            return View(todo);
        }

        // GET: Todos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Todos/Create
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
                await _cache.RemoveAsync(_TodoItemsCacheKey);
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        // GET: Todos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todo.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            return View(todo);
        }

        // POST: Todos/Edit/5
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
                    await _cache.RemoveAsync(GetTodoItemCacheKey(todo.ID));
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todo
                .FirstOrDefaultAsync(m => m.ID == id);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
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
                await _cache.RemoveAsync(GetTodoItemCacheKey(todo.ID));
                await _cache.RemoveAsync(_TodoItemsCacheKey);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TodoExists(int id)
        {
            return _context.Todo.Any(e => e.ID == id);
        }

        private string GetTodoItemCacheKey(int? id)
        {
            return _TodoItemsCacheKey+"_&_"+id;
        }
    }

    
}
