using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.OrderVMs
{
    public class OrderDetailsVM
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        public string ProductName { get; set; } 
        public Guid CompanyId { get; set; }
        public AdminDTO Admin { get; set; }
        public CompanyDTO Company { get; set; }
    }
}