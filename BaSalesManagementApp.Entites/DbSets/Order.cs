
namespace BaSalesManagementApp.Entites.DbSets
{
    public class Order : AuditableEntity
    {
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid AdminId { get; set; }
        //Admin ile ilişkisi kuruldu.
        public virtual Admin Admin { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
