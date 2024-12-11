using BaSalesManagementApp.Dtos.ProductDTOs;

namespace BaSalesManagementApp.MVC.Models.StockVMs
{
    public class StockCreateVM
    {
        public int? Count { get; set; }
        public Guid ProductId { get; set; }
        public List<ProductDTO>? Products { get; set; }
    }
}
