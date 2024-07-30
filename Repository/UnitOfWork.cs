using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Repository.IRepository;

namespace DotNetCoreSqlDb.Repository
{
    public class UnitOfWork : IUnitOFWork
    {
        public ICategoryRepository Category { get; private set; }
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
