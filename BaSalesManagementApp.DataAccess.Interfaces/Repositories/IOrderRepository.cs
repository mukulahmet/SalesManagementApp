namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IOrderRepository : IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<Order>, IAsyncDeletableRepository<Order>, IAsyncFindableRepository<Order>, IAsyncInsertableRepository<Order>, IAsyncOrderableRepository<Order>, IAsyncQueryableRepository<Order>, IDeletableRepository<Order>
    {
        /// <summary>
        ///  Orders ile Admin tablosunu birleştirerek siparişleri listeleme.
        /// </summary>
        /// <returns></returns>
        Task<List<Order>> GetAllWithAdminAsync();
    }
}
