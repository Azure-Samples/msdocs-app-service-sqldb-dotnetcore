using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Models;

namespace DotNetCoreSqlDb.Data
{
    public class TodoDb : DbContext
    {
        public TodoDb (DbContextOptions<TodoDb> options)
            : base(options)
        {
        }

        public DbSet<Todo> Todo { get; set; } = default!;
    }
}
