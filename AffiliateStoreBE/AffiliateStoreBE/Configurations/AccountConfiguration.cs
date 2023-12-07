using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AffiliateStoreBE.Models;

namespace AffiliateStoreBE.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("account-data");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.UserName).IsRequired();
            builder.Property(a => a.Email).IsRequired();
            builder.Property(a=> a.EmailConfirmed).IsRequired();
            builder.Property(a => a.PasswordHash).IsRequired();
            builder.Property(a => a.Age).IsRequired();
            builder.Property(a => a.Gender).IsRequired();
            builder.Property(a => a.Country).IsRequired();
        }
    }
}