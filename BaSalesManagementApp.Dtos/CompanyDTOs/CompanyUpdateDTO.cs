namespace BaSalesManagementApp.Dtos.CompanyDTOs
{
    public class CompanyUpdateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
