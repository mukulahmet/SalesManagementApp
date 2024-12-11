namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IEmployeeRepository :
        IAsyncRepository, IRepository,
        IAsyncTransactionRepository,
        IAsyncUpdateableRepository<Employee>,
        IAsyncDeletableRepository<Employee>,
        IAsyncFindableRepository<Employee>,
        IAsyncInsertableRepository<Employee>,
        IAsyncOrderableRepository<Employee>,
        IAsyncQueryableRepository<Employee>,
        IDeletableRepository<Employee>
    {
        Task<Employee?> GetByIdentityId(string identityId);
    }
}
