using BaSalesManagementApp.Dtos.OrderDetailDTOs;

namespace BaSalesManagementApp.Dtos.OrderDTOs
{
    public class OrderUpdateDTO
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderDetailUpdateDTO> OrderDetails { get; set; } = new List<OrderDetailUpdateDTO>();
    }
}
