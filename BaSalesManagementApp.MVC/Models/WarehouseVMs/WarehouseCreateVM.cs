using BaSalesManagementApp.Dtos.BranchDTOs;

namespace BaSalesManagementApp.MVC.Models.WarehouseVMs
{
    public class WarehouseCreateVM
    {
        public string? Name { get; set; }
        public string? Address { get; set; }

        public Guid BranchId { get; set; }
        public List<BranchDTO>? Branches { get; set; }
    }
}
