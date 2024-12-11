

namespace BaSalesManagementApp.MVC.Models.CompanyVMs
{
    public class CompanyUpdateVM
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = null!;
        public string? Address { get; set; } = null!;
        public string? Phone { get; set; } = null!;
       
    }
}
