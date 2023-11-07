using AffiliateStoreBE.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AffiliateStoreBE.Configurations
{
    public class VideoReviewConfiguration : IEntityTypeConfiguration<VideoReview>
    {
        public void Configure(EntityTypeBuilder<VideoReview> builder)
        {
            builder.ToTable("videosreview-data");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Title).IsRequired();
            builder.Property(a => a.Description).IsRequired();
            builder.Property(a => a.VideoLink).IsRequired();
            builder.Property(a => a.ProductId).IsRequired();
            builder.HasOne(a => a.Product).WithMany().HasForeignKey(a => a.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(a => a.Status).IsRequired();
        }
    }
}
