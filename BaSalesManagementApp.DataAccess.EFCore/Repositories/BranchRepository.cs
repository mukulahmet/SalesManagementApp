namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class BranchRepository : EFBaseRepository<Branch>, IBranchRepository
    {
        public BranchRepository(BaSalesManagementAppDbContext context) : base(context)
        {

        }
    }
}
