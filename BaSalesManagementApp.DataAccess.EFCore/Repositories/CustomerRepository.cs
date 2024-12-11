namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class CustomerRepository : EFBaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(BaSalesManagementAppDbContext context) : base(context)
        {
            
        }
    }
}
