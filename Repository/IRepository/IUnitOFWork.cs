namespace DotNetCoreSqlDb.Repository.IRepository
{
    public interface IUnitOFWork
    {
        ICategoryRepository Category { get; }
        void Save();
    }
}
