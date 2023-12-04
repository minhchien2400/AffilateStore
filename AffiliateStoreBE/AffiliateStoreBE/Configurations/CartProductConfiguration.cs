using AffiliateStoreBE.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AffiliateStoreBE.Configurations
{
    public class CartProductConfiguration : IEntityTypeConfiguration<CartProduct>
    {
        public void Configure(EntityTypeBuilder<CartProduct> builder)
        {
            builder.ToTable("cartproduct-data");
            builder.HasKey(a => a.CartProductId);
            builder.Property(a => a.CartProductId).ValueGeneratedOnAdd();
            builder.Property(a => a.ProductId).IsRequired();
            builder.HasOne(a => a.Product).WithMany().HasForeignKey(a => a.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(a => a.CartId).IsRequired();
            builder.HasOne(a => a.Cart).WithMany().HasForeignKey(a => a.CartId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}