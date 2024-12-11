using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface ICustomerRepository : IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<Customer>, IAsyncDeletableRepository<Customer>, IAsyncFindableRepository<Customer>, IAsyncInsertableRepository<Customer>, IAsyncOrderableRepository<Customer>, IAsyncQueryableRepository<Customer>, IDeletableRepository<Customer>
    {
    }
}
