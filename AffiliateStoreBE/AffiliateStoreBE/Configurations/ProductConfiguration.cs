using AffiliateStoreBE.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AffiliateStoreBE.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products-data");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Name).HasMaxLength(128).IsRequired();
            builder.Property(a => a.Description).IsRequired();
            builder.Property(a => a.Cost).IsRequired();
            builder.Property(a => a.Price).IsRequired();
            builder.Property(a => a.Images).IsRequired();
            builder.Property(a => a.CategoryId).IsRequired();
            builder.Property(a => a.Stars).IsRequired();
            builder.Property(a => a.AffLink).IsRequired();
            builder.Property(a => a.TotalSales).IsRequired();
            builder.HasOne(a => a.Category).WithMany().HasForeignKey(a => a.CategoryId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(a => a.Status).IsRequired();
        }
    }
}