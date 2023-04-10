namespace DataAccess.UnitOfWork.Base
{
    public interface IBaseUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(string transactionBy);
    }
}
