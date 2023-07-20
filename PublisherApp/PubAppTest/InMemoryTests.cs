using Microsoft.EntityFrameworkCore;
using PublisherConsole;
using PublisherConsole.Dtos;
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

        [TestMethod]
        public void InsertAuthorsReturnsCorrectResultNumber()
        {
            var builder = new DbContextOptionsBuilder<PubContext>();
            builder.UseInMemoryDatabase(_connectionString);

            var authorList = new List<ImportAuthorDto>() 
            {
                new ImportAuthorDto("a", "b"),
                new ImportAuthorDto("c", "d"),
                new ImportAuthorDto("e", "f"),
            };

            var dl = new DataLogic(new PubContext(builder.Options));
            var result = dl.ImportAuthors(authorList);

            Assert.AreEqual(authorList.Count, result);
        }
    }
}
