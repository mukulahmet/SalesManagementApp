namespace BaSalesManagementApp.Dtos.CompanyDTOs
{
    public class CompanyListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}
