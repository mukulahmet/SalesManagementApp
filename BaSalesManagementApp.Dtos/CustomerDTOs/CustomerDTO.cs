namespace BaSalesManagementApp.Dtos.CustomerDTOs
{
    public class CustomerDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public Guid CityId { get; set; }
        public string CityName { get; set; } = null!;
        public Guid CountryId { get; set; }

        public string CountryName { get; set; } = null!;
    }
}
