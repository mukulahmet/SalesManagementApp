using BaSalesManagementApp.Dtos.CountryDTOs;

namespace BaSalesManagementApp.MVC.Models.CityVMs
{
    public class CityUpdateVM
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } 

        public Guid? CountryId { get; set; }

        public List<CountryDTO> Countries { get; set; } = new List<CountryDTO>();

    }
}
