using BaSalesManagementApp.Dtos.ProductDTOs;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.OrderVMs
{
    public class OrderCreateVM
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        [Display(Name = "Discount Rate")]
        public decimal DiscountRate { get; set; }
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        public Guid AdminId { get; set; }
        public List<ProductListDTO> Products { get; set; } = new List<ProductListDTO>();
    }
}