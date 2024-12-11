using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.MVC.Models.PromotionVMs
{
    public class PromotionCreateVM
    {
        public int? Discount { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid? ProductId { get; set; }
        public IEnumerable<Product>? Products { get; set; } = new List<Product>();

        public Guid? CompanyId { get; set; }
        public IEnumerable<Company>? Companies { get; set; } = new List<Company>();
    }
}
