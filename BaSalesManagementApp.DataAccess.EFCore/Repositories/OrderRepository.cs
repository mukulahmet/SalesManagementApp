
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class OrderRepository : EFBaseRepository<Order>, IOrderRepository
    {
        private readonly BaSalesManagementAppDbContext _context;
        public OrderRepository(BaSalesManagementAppDbContext context) : base(context)
        {
            _context = context;
        }
        /// <summary>
        /// Orders ile Admin tablosunu birleştirerek siparişleri listeler. Bu listede Admin bilgisini de göstermesini sağlar.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Order>> GetAllWithAdminAsync()
        {
            var orders = await _context.Orders.Where(x=> x.Status!=Core.Enums.Status.Deleted).Include(o => o.Admin).ToListAsync();
            if (orders == null || !orders.Any())
            {
                Console.WriteLine("No orders found.");
            }
            else
            {
                foreach (var order in orders)
                {
                    Console.WriteLine($"Order ID: {order.Id}, Admin: {order.Admin?.FirstName} {order.Admin?.LastName}");
                }
            }
            return orders;
        }
    }
}
