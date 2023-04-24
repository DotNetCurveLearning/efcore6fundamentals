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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>()
                .HasData(new Author { Id = 1, FirstName = "Rhoda", LastName = "Lerman" });

            var authorList = new Author[]
            {
                new Author { Id = 2, FirstName = "Ruth", LastName = "Ozeki" },
                new Author { Id = 3, FirstName = "Sofia", LastName = "Segovia" },
                new Author { Id = 4, FirstName = "Ursula K.", LastName = "LeGuin" },
                new Author { Id = 5, FirstName = "Hugh", LastName = "Howey" },
                new Author { Id = 6, FirstName = "Isabelle", LastName = "Allende" }
            };

            modelBuilder.Entity<Author>()
                .HasData(authorList);
        }
    }
}