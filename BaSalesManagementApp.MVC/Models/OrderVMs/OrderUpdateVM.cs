using BaSalesManagementApp.Dtos.ProductDTOs;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.OrderVMs
{
    public class OrderUpdateVM
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        [Display(Name = "Discount Rate")]
        public decimal DiscountRate { get; set; }
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        public List<ProductListDTO>? Products { get; set; }
    }
}