namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class CategoryRepository : EFBaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(BaSalesManagementAppDbContext context) : base(context)
        {
        }
    }
}
