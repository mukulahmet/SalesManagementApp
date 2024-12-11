namespace BaSalesManagementApp.MVC.Models.CompanyVMs
{
    public class CompanyDetailsVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? CountryCode { get; set; } = null!;
    }
}
