using Microsoft.EntityFrameworkCore;
using PublisherData;
using PublisherDomain;
using System.Diagnostics;

namespace PubAppTest
{
    [TestClass]
    public class InMemoryTests
    {
        private readonly string _connectionString = Guid.NewGuid().ToString();  
        
        [TestMethod]
        public void CanInsertAuthorIntoDatabase()
        {
            var builder = new DbContextOptionsBuilder<PubContext>();

            builder.UseInMemoryDatabase(_connectionString);

            using (var context = new PubContext(builder.Options))
            {
                var author = new Author { FirstName = "a", LastName = "b" };

                context.Authors.Add(author);

                Debug.WriteLine($"Before save: {author.AuthorId}");
                context.SaveChanges();
                Debug.WriteLine($"After save: {author.AuthorId}");

                Assert.AreEqual(EntityState.Unchanged, context.Entry(author).State);
            }
        }
    }
}
