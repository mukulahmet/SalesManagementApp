﻿namespace BaSalesManagementApp.Dtos.ProductDTOs
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public byte[]? QRCode { get; set; }
        public string CategoryName { get; set; }

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;   
    }
}
