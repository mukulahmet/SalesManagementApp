namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class EmployeeRepository : EFBaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(BaSalesManagementAppDbContext context) : base(context) { }
        public Task<Employee?> GetByIdentityId(string identityId)
        {
            return _table.FirstOrDefaultAsync(x => x.IdentityId == identityId);
        }
    }
}
