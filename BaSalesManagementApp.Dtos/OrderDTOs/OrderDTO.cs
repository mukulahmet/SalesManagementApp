using BaSalesManagementApp.Dtos.OrderDetailDTOs;

namespace BaSalesManagementApp.Dtos.OrderDTOs
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid AdminId { get; set; }
        public Guid ProductId { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
