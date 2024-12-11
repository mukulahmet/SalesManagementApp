namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class AdminRespository: EFBaseRepository<Admin>,IAdminRepository
    {
        public AdminRespository(BaSalesManagementAppDbContext context):base(context) { }

        public Task<Admin?> GetByIdentityId(string identityId)
        {
            return _table.FirstOrDefaultAsync(x => x.IdentityId == identityId);
        }
    }
}
