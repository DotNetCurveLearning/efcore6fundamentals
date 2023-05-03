using Microsoft.EntityFrameworkCore;
using PublisherConsole.Aspects;
using PublisherConsole.Interfaces;
using PublisherData;
using PublisherData.Extensions;
using PublisherDomain;
using Serilog;

namespace PublisherConsole;

[CustomLog]
public class EFCoreDemo : IDataDisplayer
{
    private readonly PubContext _dbContext;

    public EFCoreDemo(PubContext dbContext)
    {
        if (dbContext is null)
        {
            throw new ArgumentNullException(nameof(dbContext));
        }

        _dbContext = dbContext;
    }

    public void DisplayData<T>(T item)
    {
        Console.WriteLine(item);
    }

    public Author FindThatAuthor(int authorId)
    {
        return _dbContext.Authors.Find(authorId);
    }

    public void QueryAggregate(string param)
    {
        var author = _dbContext.Authors
        .FirstOrDefault(author => author.LastName.Equals(param));

        var author1 = _dbContext.Authors
            .OrderBy(author => author.FirstName)
            .LastOrDefault(author => author.LastName.Equals(param));
    }

    public void SortAuthors()
    {
        var authorsByLastName = _dbContext.Authors
        .OrderByDescending(author => author.LastName)
        .ThenByDescending(author => author.FirstName)
        .ToList();

        authorsByLastName.ForEach(author => DisplayData(author));
    }

    public void SkipAndTakeAuthors()
    {
        var groupSize = 2;
        for (int i = 0; i < 5; i++)
        {
            var authors = _dbContext.Authors
                .Skip(groupSize * i)
                .Take(groupSize)
                .ToList();

            DisplayData($"Group {i}:");

            foreach (var author in authors)
            {
                DisplayData(author);
            }
        }
    }

    public void AddAuthorWithBooks()
    {
        var author = new Author { FirstName = "Julie", LastName = "Colman" };
        author.Books.Add(new Book { Title = "Programming Entity Framework", PublishDate = new DateTime(2009, 1, 1) });
        author.Books.Add(new Book { Title = "Programming Entity Framework 2nd Ed", PublishDate = new DateTime(2010, 8, 1) });

        _dbContext.Authors.AddIfNotExists(author, a => a.FirstName == author.FirstName);
        _dbContext.SaveChanges();
    }

    public void AddSomeMoreAuthors()
    {
        _dbContext.Authors.Add(new Author { FirstName = "Rhoda", LastName = "Lerman" });
        _dbContext.Authors.Add(new Author { FirstName = "Don", LastName = "Jones" });
        _dbContext.Authors.Add(new Author { FirstName = "Jim", LastName = "Christopher" });
        _dbContext.Authors.Add(new Author { FirstName = "Stephen", LastName = "Haunts" });

        _dbContext.SaveChanges();
    }
    public void AddAuthor()
    {
        var author = new Author { FirstName = "Ernesto", LastName = "Antonio" };

        _dbContext.Authors.AddIfNotExists(author, a => a.FirstName == author.FirstName);
        _dbContext.SaveChanges();
    }

    public void GetAuthorsWithBooks()
    {
        var authors = _dbContext.Authors.Include(author => author.Books).ToList();

        foreach (var author in authors)
        {
            DisplayData($"{author.FirstName} {author.LastName}");

            foreach (var book in author.Books)
            {
                DisplayData($"* {book}");
            }
        }
    }

    public void GetAuthors()
    {
        var authors = _dbContext.Authors.ToList();

        foreach (var author in authors)
        {
            DisplayData($"{author.FirstName} {author.LastName}");
        }
    }

    public void QueryingFiltersWithParametersAndLike(string param)
    {
        var authors = _dbContext.Authors
            .Where(author => EF.Functions.Like(author.LastName, $"%{param}%"))
            .ToList();

        foreach (var author in authors)
        {
            DisplayData($"{author.FirstName} {author.LastName}");
        }
    }

    public void QueryingFiltersWithParameters(string firstName)
    {
        var authors = _dbContext.Authors
            .Where(author => author.FirstName.Equals(firstName))
            .ToList();

        foreach (var author in authors)
        {
            DisplayData(author);
        }
    }

    public void DeleteAnAuthor()
    {
        var extraJL = _dbContext.Authors.Find(1);

        if (extraJL != null)
        {
            _dbContext.Authors.Remove(extraJL);
            _dbContext.SaveChanges();
        }
    }

    public void CoordinatedRetrieveAndUpdateAuthor()
    {
        var author = FindThatAuthor(8);

        if (author.FirstName.Equals("Julie"))
        {
            author.FirstName = "Julia";
            SaveThatAuthor(author);
        }
    }

    public void SaveThatAuthor(Author author)
    {
        _dbContext.Authors.Update(author);
        _dbContext.SaveChanges();
    }

    public void VariousOperations()
    {
        var author = _dbContext.Authors.Find(7);
        author.LastName = "Newfoundland";

        var newAuthor = new Author { FirstName = "Dan", LastName = "Appleman" };

        _dbContext.Authors.Add(newAuthor);
        _dbContext.SaveChanges();
    }

    public void RetrieveAndUpdateMultipleAuthors()
    {
        var authors = _dbContext.Authors.
            Where(author => author.LastName.Equals("Lehrman")).ToList();

        foreach (var author in authors)
        {
            author.LastName = "Lerman";
        }

        DisplayData("Before " + _dbContext.ChangeTracker.DebugView.ShortView);
        _dbContext.ChangeTracker.DetectChanges();
        DisplayData("After " + _dbContext.ChangeTracker.DebugView.ShortView);

        _dbContext.SaveChanges();
    }

    public void RetrieveAndUpdateAuthor()
    {
        var author = _dbContext.Authors.
            FirstOrDefault(author => author.FirstName.Equals("Julie") && author.LastName.Equals("Colman"));

        if (author != null)
        {
            author.FirstName = "Julia";
            author.LastName = "Lerman";
            _dbContext.SaveChanges();
        }
    }

    public void InsertMultipleAuthors()
    {
        var newAuthors = new Author[]
            {
            new Author { FirstName = "Ruth", LastName = "Ozeki" },
            new Author { FirstName = "Sofia", LastName = "Segovia" },
            new Author { FirstName = "Ursula K.", LastName = "LeGuin" },
            new Author { FirstName = "Hugh", LastName = "Howey" },
            new Author { FirstName = "Isabelle", LastName = "Allende" }
            };

        _dbContext.AddRange(newAuthors);
        _dbContext.SaveChanges();
    }

    public void BulkAddUpdate()
    {
        var newAuthors = new Author[]
            {
            new Author { FirstName = "Ruth", LastName = "Ozeki" },
            new Author { FirstName = "Sofia", LastName = "Segovia" },
            new Author { FirstName = "Ursula K.", LastName = "LeGuin" },
            new Author { FirstName = "Hugh", LastName = "Howey" },
            new Author { FirstName = "Isabelle", LastName = "Allende" }
            };

        _dbContext.AddRange(newAuthors);

        var book = _dbContext.Books.Find(2);
        book.Title = "Programming Entity Framework 2nd Edition";

        _dbContext.SaveChanges();
    }

    public void FindAuthorByKey(int entityKey)
    {
        var author = _dbContext.Authors.Find(entityKey);
        DisplayData(author);
    }

    public void InsertNewAuthorWithNewBook()
    {
        var author = new Author { FirstName = "Lynda", LastName = "Rutledge" };
        author.Books.Add(new Book
        {
            Title = "West With Giraffes",
            PublishDate = new DateTime(2021, 2, 1)
        });

        _dbContext.Authors.Add(author);
        _dbContext.SaveChanges();
    }

    public void InsertNewAuthorWith2Book()
    {
        var author = new Author { FirstName = "Don", LastName = "Jones" };
        author.Books.AddRange(new List<Book>
        {
            new Book { Title = "The Never", PublishDate = new DateTime(2019, 12, 1) },
            new Book { Title = "Alabaster", PublishDate = new DateTime(2019, 4, 1) },
        });

        _dbContext.Authors.Add(author);
        _dbContext.SaveChanges();
    }

    public void AddNewBookToExistingAuthorInMemory()
    {
        var author = _dbContext.Authors.FirstOrDefault(a => a.LastName == "Howey");

        if (author is not null)
        {
            author.Books.Add(new Book
            {
                Title = "Wool",
                PublishDate = new DateTime(2021, 1, 1)
            });
        }

        _dbContext.SaveChanges();
    }

    public void AddNewBookToExistingAuthorInMemoryViaBook()
    {
        var book = new Book 
        { 
            Title = "Shift", 
            PublishDate = new DateTime(2012, 1, 1),
            AuthorId = 5,
        };
        
        //book.Author = _dbContext.Authors.Find(5);

        _dbContext.Books.Add(book);
        _dbContext.SaveChanges();
    }
}
