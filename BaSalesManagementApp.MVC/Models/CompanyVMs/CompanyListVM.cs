namespace BaSalesManagementApp.MVC.Models.CompanyVMs
{
    public class CompanyListVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? CountryCode { get; set; } = null!;

    }
}
