namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class StockRepository : EFBaseRepository<Stock>, IStockRepository
    {
        public StockRepository(BaSalesManagementAppDbContext context) : base(context)
        {

        }
    }
}
