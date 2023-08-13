using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Models;

namespace DotNetCoreSqlDb.Data
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext (DbContextOptions<CoreDbContext> options)
            : base(options)
        {
        }       

        public DbSet<DotNetCoreSqlDb.Models.EMASignal> EMASignal { get; set; } = default!;
    }
}
