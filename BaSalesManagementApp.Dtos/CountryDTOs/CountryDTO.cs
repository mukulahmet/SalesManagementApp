using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.CountryDTOs
{
    public class CountryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;  
    }
}
