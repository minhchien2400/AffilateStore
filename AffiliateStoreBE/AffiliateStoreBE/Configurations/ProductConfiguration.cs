using AffiliateStoreBE.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AffiliateStoreBE.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Name).HasMaxLength(128).IsRequired();
            builder.Property(a => a.Description).IsRequired();
            builder.Property(a => a.Price).HasMaxLength(32).IsRequired();
            builder.Property(a => a.Type).HasMaxLength(128).IsRequired();
        }
    }
}