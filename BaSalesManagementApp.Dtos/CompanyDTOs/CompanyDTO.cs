using System.ComponentModel;

namespace BaSalesManagementApp.Dtos.CompanyDTOs
{
    public class CompanyDTO
    {
        public Guid Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; } = null!;
        [DisplayName("Address")]
        public string Address { get; set; } = null!;
        [DisplayName("Phone")]
        public string Phone { get; set; } = null!;
    }
}
