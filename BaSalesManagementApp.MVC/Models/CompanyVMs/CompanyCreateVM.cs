
using System.ComponentModel.DataAnnotations;


namespace BaSalesManagementApp.MVC.Models.CompanyVMs
{
    public class CompanyCreateVM
    {
        public string? Name { get; set; } = null!;
        public string? Address { get; set; } = null!;
        
        public string? Phone { get; set; } = null!;

		public string? CountryCode { get; set; } = null!;
		
	}
}
