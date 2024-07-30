using DotNetCoreSqlDb.Models;

namespace DotNetCoreSqlDb.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category> GetCategoryById(int id);
    }
}
