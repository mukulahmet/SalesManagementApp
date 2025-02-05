﻿using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.MVC.Models.CustomerVMs
{
    public class CustomerCreateVM
    {
        public string? Name { get; set; } = null!;
        public string? Address { get; set; } = null!;
        public string? Phone { get; set; } = null!;
        public Guid? CompanyId { get; set; }
        public Guid? CityId { get; set; }
        public Guid? CountryId { get; set; }
        public List<Company>? Companies { get; set; } = new List<Company>();
        public List<City> Cities { get; set; } = new List<City>();
        public List<Country> Countries { get; set; } = new List<Country>();
    }
}
