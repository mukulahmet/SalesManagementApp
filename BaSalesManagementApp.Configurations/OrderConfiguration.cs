namespace BaSalesManagementApp.Entities.Configurations
{
    public class OrderConfiguration : AuditableEntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.Quantity).IsRequired();
            builder.Property(x => x.TotalPrice).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.OrderDate).IsRequired();
            builder.HasMany(x => x.OrderDetails)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId);
            base.Configure(builder);
        }
    }
}
