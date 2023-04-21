using Microsoft.EntityFrameworkCore;
using PublisherData;
using PublisherData.Extensions;
using PublisherDomain;

// Not needed anymore since the db is created already
/*using (PubContext context = new PubContext())
{
    // The EnsureCreated method cause EF core read the provider 
    // and connection string defined in the PubContext class and
    // then go look to see if the db exists. If not, it will be created.
    context.Database.EnsureCreated();
}*/

// this assumes we're working with the populated database already created
PubContext _context = new PubContext();

//GetAuthors();
//AddAuthor();
//AddAuthorWithBooks();
//GetAuthorsWithBooks();
//QueryingFiltersWithParameters("Josie");
// QueryingFiltersWithParametersAndLike("L");
//AddSomeMoreAuthors();
//SkipAndTakeAuthors();
//FindAuthorByKey(8);
//SortAuthors();
QueryAggregate("Lerman");


void FindAuthorByKey(int entityKey)
{
    var author = _context.Authors.Find(entityKey);
    DisplayAuthorData(author);
}

void QueryingFiltersWithParameters(string firstName)
{
    var authors = _context.Authors
        .Where(author => author.FirstName.Equals(firstName))
        .ToList();

    foreach (var author in authors)
    {
        DisplayAuthorData(author);
    }
}

void QueryingFiltersWithParametersAndLike(string param)
{
    var authors = _context.Authors
        .Where(author => EF.Functions.Like(author.LastName, $"%{param}%"))
        .ToList();

    foreach (var author in authors)
    {
        Console.WriteLine($"{author.FirstName} {author.LastName}");
    }
}

void GetAuthors()
{
    var authors = _context.Authors.ToList();

    foreach (var author in authors)
    {
        Console.WriteLine($"{author.FirstName} {author.LastName}");
    }
}

void GetAuthorsWithBooks()
{
    using var context = new PubContext();
    var authors = context.Authors.Include(author => author.Books).ToList();

    foreach (var author in authors)
    {
        Console.WriteLine($"{author.FirstName} {author.LastName}");

        foreach (var book in author.Books)
        {
            Console.WriteLine($"* {book.Title}");
        }
    }
}
void AddAuthor()
{
    var author = new Author { FirstName = "Ernesto", LastName = "Antonio" };

    _context.Authors.AddIfNotExists(author, a => a.FirstName == author.FirstName);
    _context.SaveChanges();
}

void AddSomeMoreAuthors()
{
    _context.Authors.Add( new Author { FirstName = "Rhoda", LastName = "Lerman" });
    _context.Authors.Add(new Author { FirstName = "Don", LastName = "Jones" });
    _context.Authors.Add(new Author { FirstName = "Jim", LastName = "Christopher" });
    _context.Authors.Add(new Author { FirstName = "Stephen", LastName = "Haunts" });
    
    _context.SaveChanges();
}

void AddAuthorWithBooks()
{
    var author = new Author { FirstName = "Julie", LastName = "Colman" };
    author.Books.Add(new Book { Title = "Programming Entity Framework", PublishDate = new DateTime(2009, 1, 1) });
    author.Books.Add(new Book { Title = "Programming Entity Framework 2nd Ed", PublishDate = new DateTime(2010, 8, 1) });

    _context.Authors.AddIfNotExists(author, a => a.FirstName == author.FirstName);
    _context.SaveChanges();
}

void SkipAndTakeAuthors()
{
    var groupSize = 2;
    for (int i = 0; i < 5; i++)
    {
        var authors = _context.Authors
            .Skip(groupSize * i)
            .Take(groupSize)
            .ToList();

        Console.WriteLine($"Group {i}:");

        foreach (var author in authors)
        {
            DisplayAuthorData(author);
        }
    }
}

void SortAuthors()
{
    //var authorsByLastName = _context.Authors
    //    .OrderBy(author => author.LastName)
    //    .ThenBy(author => author.FirstName)
    //    .ToList();

    var authorsByLastName = _context.Authors
        .OrderByDescending(author => author.LastName)
        .ThenByDescending(author => author.FirstName)
        .ToList();

    authorsByLastName.ForEach(author => DisplayAuthorData(author));
}

void QueryAggregate(string param)
{
    var author = _context.Authors
        .FirstOrDefault(author => author.LastName.Equals(param));
    
    var author1 = _context.Authors
        .OrderBy(author => author.FirstName)
        .LastOrDefault(author => author.LastName.Equals(param));

    DisplayAuthorData(author);
    DisplayAuthorData(author1);
}

static void DisplayAuthorData(Author? author)
{
    Console.WriteLine($"{author?.LastName} {author?.FirstName}");
}