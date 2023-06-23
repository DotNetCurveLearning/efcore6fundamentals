using Microsoft.EntityFrameworkCore;
using PublisherConsole.Interfaces;
using PublisherData;
using PublisherData.Extensions;
using PublisherDomain;
using System.Text;

namespace PublisherConsole;

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

    public void InsertNewAuthorWithBooks(Author author, List<Book> books)
    {
        author.Books.AddRange(books);

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

    public void EagerLoadBooksWithAuthors()
    {
        var pubDateStart = new DateTime(2010, 1, 1);
        var authors = _dbContext.Authors
            .Include(a => a.Books.Where(b => b.PublishDate >= pubDateStart).OrderBy(b => b.Title))
            .ToList();

        authors.ForEach(author =>
        {
            Console.WriteLine($"{author.LastName} ({author.Books.Count})");
            author.Books.ForEach(book => Console.WriteLine(DisplayBookData(book)));
        });
    }

    public void Projections()
    {
        var unknownTypes = _dbContext.Authors
            .Select(author => new
            {
                AuthorId = author.AuthorId,
                Name = new StringBuilder().Append(author.FirstName).Append(" ").Append(author.LastName).ToString(),
                // Books = author.Books.Count
                Books = author.Books.Where(book => book.PublishDate.Year < 2000).Count()
            })
            .ToList();

        unknownTypes.ForEach(item => Console.WriteLine(item));
        var debugView = _dbContext.ChangeTracker.DebugView.ShortView;
    }

    public void ExplicitLoadCollections()
    {
        var author = _dbContext.Authors.FirstOrDefault(a => a.LastName.Equals("Howey"));
        _dbContext.Entry(author).Collection(a => a.Books).Load();
    }

    public void LazyLoadingFromAnAuthor()
    {
        // requires lazy loading to be set up in the app
        var author = _dbContext.Authors.FirstOrDefault(a => a.LastName.Equals("Howey"));

        foreach (var book in author.Books)
        {
            Console.WriteLine(book.Title);
        }
    }

    public void FilterUsingRelatedData()
    {
        var recentAuthors = _dbContext.Authors
            .Where(author => author.Books.Any(book => book.PublishDate.Year >= 2015))
            .ToList();
    }

    public void ModifyingRelatedDateWhenTracked()
    {
        var author = _dbContext.Authors.Include(a => a.Books)
            .FirstOrDefault(a => a.AuthorId == 5);

        author.Books[0].BasePrice = (decimal)10.00;

        _dbContext.ChangeTracker.DetectChanges();
        var state = _dbContext.ChangeTracker.DebugView.ShortView;
    }

    public void CascadeDeleteInActionWhenTracked()
    {
        var author = _dbContext.Authors.Include(a => a.Books)
            .FirstOrDefault(a => a.AuthorId == 1009);
        _dbContext.Remove(author);

        var state = _dbContext.ChangeTracker.DebugView.ShortView;
        _dbContext.SaveChanges();
    }

    public void ConnectExistingArtistAndCoverObjects()
    {
        var artistA = _dbContext.Artists.Find(1);
        var artistB = _dbContext.Artists.Find(2);

        var coverA = _dbContext.Covers?.Find(1);
        coverA?.Artists.Add(artistA);
        coverA?.Artists.Add(artistB);

        _dbContext.SaveChanges();
    }

    public void CreateNewCoverWithExistingArtist()
    {
        var artistA = _dbContext.Artists.Find(1);
        var cover = new Cover
        {
            DesignIdeas = "Author has provided a photo",
            DigitalOnly = true
        };

        cover.Artists.Add(artistA);
        _dbContext.Covers.Add(cover);

        _dbContext.SaveChanges();
    }

    public void CreateNewCoverAndArtistTogether()
    {
        var newArtist = new Artist
        {
            FirstName = "Kir",
            LastName = "Talmage"
        };

        var newCover = new Cover
        {
            DesignIdeas = "We like birds!",
            DigitalOnly = false
        };

        newArtist.Covers.Add(newCover);
        _dbContext.Artists.Add(newArtist);

        _dbContext.SaveChanges();
    }

    public void RetrieveAnArtistWithTheirCovers()
    {
        var artistWithCovers = _dbContext.Artists
            .Include(a => a.Covers)
            .FirstOrDefault(a => a.ArtistId == 1);

        Console.WriteLine(artistWithCovers);
        artistWithCovers?.Covers.ToList().ForEach(cover => Console.WriteLine(cover.DesignIdeas));
    }

    public void RetrieveACoverWithItsArtists()
    {
        var coverWithArtists = _dbContext.Covers
            .Include(c => c.Artists)
            .FirstOrDefault(c => c.CoverId == 1);

        Console.WriteLine(coverWithArtists);
        coverWithArtists?.Artists.ToList().ForEach(artist => Console.WriteLine(artist));
    }

    public void RetrieveAllArtistsWtihTheirCovers()
    {
        var allArtistsWithTheirCovers = _dbContext.Artists
            .Include(a => a.Covers)
            .ToList();

        foreach (var a in allArtistsWithTheirCovers)
        {
            Console.WriteLine($"{a.FirstName} {a.LastName}, Designs to work on:");
            var primaryArtistId = a.ArtistId;

            if (a.Covers?.Count() == 0)
            {
                Console.WriteLine("     No covers");
            }
            else
            {
                foreach (var c in a.Covers)
                {
                    var co = new StringBuilder().Append(String.Empty);

                    foreach (var ca in c.Artists.Where(ca => ca.ArtistId != primaryArtistId))
                    {
                        co.Append(ca.FirstName).Append(" ").Append(ca.LastName);
                    }

                    if (co.Length > 0)
                    {
                        co.Insert(0, "(with ").Append(")");
                    }

                    Console.WriteLine($"    *{c.DesignIdeas} {co}");
                    co.Clear();
                }
            }
        }
    }

    public void RetrieveAllArtistsWhoHaveCovers()
    {
        var allArtistsWithTheirCovers = _dbContext.Artists
            .Where(a => a.Covers.Any())
            .ToList();

        allArtistsWithTheirCovers.ForEach(a => Console.WriteLine(a));
    }

    public void UnAssignAnArtistFromACover()
    {
        var coverWithArtist = _dbContext.Covers
            .Include(c => c.Artists.Where(a => a.ArtistId == 1))
            .FirstOrDefault(c => c.CoverId == 1);

        if (coverWithArtist?.Artists is List<Artist> list && list.Any())
        {
            list.RemoveAt(0);
        }

        _dbContext.ChangeTracker.DetectChanges();

        var debugView = _dbContext.ChangeTracker.DebugView.ShortView;
        _dbContext.SaveChanges();
    }

    public void ReassingACover()
    {
        var coverWithArtist4 = _dbContext.Covers
            .Include(c => c.Artists.Where(a => a.ArtistId == 4))
            .FirstOrDefault(c => c.CoverId == 5);

        if (coverWithArtist4?.Artists is List<Artist> list && list.Any())
        {
            list.RemoveAt(0);
        }

        var artist3 = _dbContext.Artists.Find(3);
        coverWithArtist4?.Artists.Add(artist3);

        _dbContext.ChangeTracker.DetectChanges();
    }

    public void GetAllBooksWithTheirCovers()
    {
        var booksAndCovers = _dbContext.Books
            .Include(book => book.Cover)
            .ToList();
        
        booksAndCovers.ForEach(book =>
            Console.WriteLine(book.Title + (book.Cover == null ? ": No cover yet" : ": " + book.Cover.DesignIdeas)));        
    }

    /// <summary>
    /// Since this query is for a report and has
    /// several includes, it will be a no tracking query
    /// to avoid paying the price for all the change tracking.
    /// </summary>
    public void MultiLevelInclude()
    {
        var authorGraph = _dbContext.Authors.AsNoTracking()
            .Include(author => author.Books)
            .ThenInclude(book => book.Cover)
            .ThenInclude(cover => cover.Artists)
            .FirstOrDefault(author => author.AuthorId == 3);

        var authorCompleteName = new StringBuilder()
            .Append(authorGraph?.FirstName)
            .Append(" ")
            .Append(authorGraph?.LastName);

        Console.WriteLine(authorCompleteName.ToString());

        foreach (var book in authorGraph.Books)
        {
            Console.WriteLine("Book: " + book.Title);

            if (book.Cover != null)
            {
                Console.WriteLine("Design ideas: " + book.Cover.DesignIdeas);
                Console.Write("Artist(s): ");
                book.Cover.Artists
                    .ToList()
                    .ForEach(artist => Console.Write(artist.LastName + " "));
            }
        }
    }

    public void NewBookAndCover()
    {
        var book = new Book 
        {
            AuthorId = 1,
            Title = "Call me Ishtar",
            PublishDate = new DateTime(1973, 1, 1)
        };

        book.Cover = new Cover 
        {
            DesignIdeas = "Images of Ishtar?",
            DigitalOnly = false,
        };

        _dbContext.Books.Add(book);
        _dbContext.SaveChanges();
    }

    public void AddCoverToExistingBook()
    {
        var book = _dbContext.Books.Find(7);

        book.Cover = new Cover
        {
            DesignIdeas = "A wool scouring pad",
            DigitalOnly = false,
        };

        _dbContext.SaveChanges();
    }

    public void AddCoverToExistingBookWithTrackedCover()
    {
        var book = _dbContext.Books
            .Include(book => book.Cover)
            .FirstOrDefault(book => book.BookId == 5);

        var TheNeverDesignIdeas = "A spirally spiral";

        if (book.Cover != null)
        {
            book.Cover.DesignIdeas = TheNeverDesignIdeas;
        }
        else
        {
            book.Cover = new Cover
            {
                DesignIdeas = "A spirally spiral",
                DigitalOnly = false,
            };
        }

        _dbContext.SaveChanges();
    }

    public void SimpleRawSQL()
    {
        var authors = _dbContext.Authors
            .FromSqlRaw("select * from authors")
            .Include(author => author.Books)
            .ToList();

        authors.ForEach(DisplayData);
    }

    public void ConcatenatedRawSql_Unsafe()
    {
        var lastNameStart = "L";
        var authors = _dbContext.Authors
            .FromSqlRaw("SELECT * FROM authors WHERE lastname LIKE '" + lastNameStart + "%'")
            .OrderBy(a => a.LastName)
            .TagWith("Concatenated_Unsafe")
            .ToList();

        authors.ForEach(DisplayData);
    }

    public void StringFromInterpolated_Unsafe()
    {
        var lastNameStart = "L";
        var authors = _dbContext.Authors
            .FromSqlRaw($"SELECT * FROM authors WHERE lastname LIKE '{lastNameStart}%'")
            .OrderBy(a => a.LastName)
            .TagWith("Interpolated_Unsafe")
            .ToList();

        authors.ForEach(DisplayData);
    }

    public void StringFromInterpolated_Safe()
    {
        var lastNameStart = "L";
        var authors = _dbContext.Authors
            .FromSqlInterpolated($"SELECT * FROM authors WHERE lastname LIKE '{lastNameStart}%'")
            .OrderBy(a => a.LastName)
            .TagWith("Interpolated_Safe")
            .ToList();

        authors.ForEach(DisplayData);
    }

    public void RawSqlStoredProc()
    {
        var authors = _dbContext.Authors
            .FromSqlRaw("AuthorsPublishedInYearRange {0}, {1}", 2010, 2015)
            .ToList();
    }

    public void InterpolatedSqlStoredProc()
    {
        int start = 2010;
        int end = 2015;

        var authors = _dbContext.Authors
            .FromSqlInterpolated($"AuthorsPublishedInYearRange {start}, {end}")
            .ToList();
    }

    public void GetAuthorsByArtist()
    {
        var authorArtists = _dbContext.AuthorsByArtist
            .TagWith("authorArtists query")           
            .ToList();

        var oneauthorartists = _dbContext.AuthorsByArtist
            .TagWith("oneauthorartists query")
            .FirstOrDefault();

        var Kauthorartists = _dbContext.AuthorsByArtist
            .TagWith("Kauthorartists query")
            .Where(a => a.Artist.StartsWith("K"))
            .ToList();

        var debugView = _dbContext.ChangeTracker.DebugView.ShortView;
    }

    private static string DisplayBookData(Book book)
    {
        return new StringBuilder().Append("     ").Append(book.Title).ToString();
    }
}
