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
                .HasData(new Author { AuthorId = 1, FirstName = "Rhoda", LastName = "Lerman" });

            var authorList = new Author[]
            {
                new Author { AuthorId = 2, FirstName = "Ruth", LastName = "Ozeki" },
                new Author { AuthorId = 3, FirstName = "Sofia", LastName = "Segovia" },
                new Author { AuthorId = 4, FirstName = "Ursula K.", LastName = "LeGuin" },
                new Author { AuthorId = 5, FirstName = "Hugh", LastName = "Howey" },
                new Author { AuthorId = 6, FirstName = "Isabelle", LastName = "Allende" }
            };

            modelBuilder.Entity<Author>()
                .HasData(authorList);

            var someBooks = new Book[]
            {
                new Book { BookId = 1, AuthorId = 1, Title = "In God's Ear" },
                new Book { BookId = 2, AuthorId = 2, Title = "A tale for the time being" },
                new Book { BookId = 3, AuthorId = 3, Title = "The left hand of darkness" }
            };

            modelBuilder.Entity<Book>()
                .HasData(someBooks);
        }
    }
}