using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace AffiliateStoreBE.DbConnect
{
    public class StoreDbContextFactory : IDesignTimeDbContextFactory<StoreDbContext>
    {
        public StoreDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configurationRoot.GetConnectionString("StoreDatabase");

            var optionBuilder = new DbContextOptionsBuilder<StoreDbContext>();
            optionBuilder.UseSqlServer(connectionString);

            return new StoreDbContext(optionBuilder.Options);
        }
    }
}
