namespace BaSalesManagementApp.Entites.DbSets
{
    public class StockType : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}