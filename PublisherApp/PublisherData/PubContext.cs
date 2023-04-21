using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PublisherDomain;

namespace PublisherData
{
    public class PubContext : DbContext
    {
        private const string CONNECTION_STRING = "PubDatabase";
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString(CONNECTION_STRING));
        }
    }
}