# What is Entity Framework Core?

It's an evolution of Microsoft's Object Relational Mapper (ORM).

It encompasses a set of .NET APIs for performing data access in our SW, and it is the official data access platform from Microsoft and it's cross-platform.

ORMs are designed to reduce the friction between how data is structured in a relational database and how we define our classes. ORM allow us to express our queries using our classes, and then the ORM itself
builds and executes the relevant SQL for us, as well as materializing objects from the data that come back from the database.

It alos allows us to store and update and even delete dasta in the db.

# EF Core 6 Fundamentals

**Domain**: A specified sphere of activity or knowledge.

The **Microsoft.EntityFrameworkCore** package os the base package that only has commonly needed logic.

## NuGet package dependencies

EF Core SQL Server	---> EF Core Relational		---> EF Core	---> .NET 6

## Creating the Data Model with EF Core

The **DbContext** class provides EF Core's database connectivity and tracks changes to objects.

- The DbContext need to exposes a **DbSet**, that wraps the classes that EF Core will work with.

**IMPORTANT**:

EF core uses "conventions" to infer the schema of the database we are working with wether we're working with an existing database or we're going to let EF Core create the database using this model.

## Specifying the Data Provider and Connection String

We must specifiy data provider and connection string.

The **OnConfiguring** method will get called internally by EF Core as it's working out what goes in the model, and it will also pass in an **optionsBuilder** object, and we can use it to configure options for the
DbContext.

### How EF core interprets our Data Model?

DbContext + conventional and custom mappings = Database Schema

### Creating the Data Model and Database

* At runtime with code

* At design time with tools

* Create the database

### Reading and writing some data

```
using var context = new PubContext();
var authors = context.Authors.Include(author => author.Books).ToList();
```

# Using EF Core 6 to query a database

**context.Authors.ToList()**	Express and execute query. The ToList() method is what triggers EF Core to go ahead and execute the query.
	|
	|
EF Core reads model, works with provider to work out SQL
	|
	|

**SELECT * from Authors**:		Sends SQL to database. EF Core opens the connection using the connection string and provider information and
								executes the SQL command in the database.
	|
	|
Receives tabular results
	|
	|
Materializes results as objects
	|
	|
Adds tracking details to DbContext instance

All this effort is orchestrated by the **DbContext**.

## Qyerying basics

There are two ways to express LINQ queries:

1) LINQ methods

```
context.Authors.ToList();

context.Authors
	.Where(a => a.FirstName == "Julie")
	.ToList();
```

2) LINQ operators

```
(from a in context.Authors
select a)
.ToList()

(from a in context.Authors
where a.FirstName == "Julie"
select a)
.ToList()
```

## Filtering queries securely by default

Parameterized queries protect your database from SQL injection attacks.

```
var authors = _context.Authors
    .Where(author => author.FirstName.Equals(firstName))
    .ToList();
```

Any time EF Core sees a variable being used in a LINQ query, it will always build a parameterized query to send to the database.

## Benefiting from additional filtering features

**Filtering partial text in queries**

One of the modes to achieve this is to write Like queries:

```
_context.Authors.Where(a => EF.Functions.Like(a.Name, "%abc%")
```

We could achieve the same kind of filtering with the LINQ **Contains** method:

```
_context.Authors.Where(a => a.Contains("abc")
```

**Finding an entity using its Key value**

For this, we can use the **DbSet.Find)keyvalue)**. This is the only task that Find can be used for.
**Find** isnt't a LINQ method that will excute right away. It's a DbSet method that will excutes immediately.

EF core constructs a SELECT TOP(1) query to locate the first result in the database.

**Skip and take for paging**

This kind of filtering is handy for websites. LINQ supports paging with **Skip** and **Take** methods.

## Sorting data in queries

The LINQ method used for sorting is **OrderBy**. It takes a lambda expression of the property you want to sort by.

```
OrderBy(o => o.Property)
```

If we want to sort for several properties, then after the OrderBy we need to use the **ThenBy** method:

```
var authorsByLastName = _context.Authors
        .OrderBy(author => author.LastName)
        .ThenBy(author => author.FirstName)
        .ToList();
```

we can also do desceding sorts (**OrderByDescending** method) and, if needed, **ThenByDescending** method. 

## Aggregating results in queries

The LINQ aggregating methods constraint the SQL query with some means of aggregating:

First()						FirstAsync()
FirstOrDefault()			FirstOrDefaultAsync()
Single()					SingleAsync()
SingleOrDefault()			SingleOrDefaultAsync()
Last()						LastAsync()
LastOrDefault()				LastOrDefaultAsync()
Count()						CountAsync()
LongCount()					LongCountAsync()
Min(), Max()				MinAsync(), MaxAsync()
Average(), Sum()			AverageAsync(), SumAsync()

**No Aggregation**

ToList()					ToListAsync()
AsEnumerable()				AsAsyncEnumerable()

*NOTE*: **Last** methods require query to have an **OrderBy()** method. Otherwise, will throw an exception.

## Enhancing query performance when tracking isnt' needed

Not every scenario requires that we keep track of the state of the objects. For example, if we're presenting something for display only and the user won't be editing, there's no need for EF Core to waste its time and
resources creating those little tracking objects, the EntityEntries objects, to manage the state of the query results.

**Change tracking is expensive**.

The most common scenario in which the default tracking is a waste is web and mobile applications. We refer to these as disconnected apps because the application or web browser running in our computer or device are
disconnected from the server.

EF core has two ways to avoid tracking when it's a waste of resources:

1) **No track queries and DbContexts** : It's a really important pattern to use when building websites or web APIs, for example.

```
var author = _context.Authors
	.AsNotTracking()
	.FirstOrDefault();
```

**AsNotTracking()** returns a query, not a DbSet**.

2) The other way is one that can be applied to a DbContext to ensure that by default any query executed by that context doesn't track.

```
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
	optionsBuilder
	.UseSqlServer(myconnectionString)
	.UseQueryTrackingBehavior(QueryTrackingBehavior.NotTracking);
}
```

**All queries for this DbContext will default to not tracking**.

# Chapter 04 - Tracking and saving data with EF Core

## Gaining a better understanding of DbContext and Entity

The **DbContext** is a representation of a session with the database, and that session begins not when the context is instantiated,
but the first time we ask the context to do something with the database: execute query or save some data.

Its **ChangeTracker** is a critical piece of that. It manages a collection of **EntityEntry** objects. The ChangeTracker creates
them and maintains their values.

The **EntityEntry** state info for each entity: CurrentValues, OriginalValues, State enum, Entity & more.

The **Entity** within the context of EF Core, have a particular meaning. It's an in-memory object with key (identity) properties
that the DbContext is aware of.

## Tracking and saving workflow

EF Core creates tracking objects for each entity:

The tracking states are:
* Unchanged (default)
* Added
* Modified
* Deleted

When we call the **SaveChanges** method from DbContext, it will updates the state for each entity, and works with provider to 
compose SQL statements, and then, executes statements on database.

## Inserting simple objects

The objects can be added from DbSet or DbContext.

**Track via DbSet**

```
context.Authors.Add(...)
```

DbSet knows the type. DbContext knows that it's Added.

**Track via DbContext**

```
context.Add(...)
```

DbContext will discover type. Knows it's Added.

## Updating simple objects

```
void RetrieveAndUpdateAuthor()
{
    var author = _context.Authors.
        FirstOrDefault(author => author.FirstName.Equals("Julie") && author.LastName.Equals("Lerman"));

    if (author != null )
    {
        author.FirstName = "Julia";
        _context.SaveChanges();
    }
}
```

How DbContext discover about changes?

That's done by **DbContext.ChangeTracker.DetectChanges**. It reads each object being tracked and updates state details in 
EntityEntry object.

DbContext.SaveChanges() always calls DetectChanges for you.

**The entities are just plain objects and don't communicate their changes to the DbContext**.

## Updating untracked objects

Most commonly, this will happen in a disconnected application, like a web app.

If we get an instance of the DbContext through the keyword **using**, the DbContext will get disposed at the end of the method that
instantiated it.

So, if the context not longer exists, there is nothing tracking that entity.

In those cases, since the entity is untracked, we need to use the **Update()** method from DbContext, because it does two things:
first, it causes the context to begin tracking the entity, and at the same time, it instructs the context to set the state for
that object to **Modified** for all the entity's properties.

It's theh only way to be sure that we're getting all of our changes to the database.

## Deleting simple objects

```
void DeleteAnAuthor()
{
    var extraJL = _context.Authors.Find(1);

    if (extraJL != null)
    {
        _context.Authors.Remove(extraJL);
        _context.SaveChanges();
    }
}
```

## Tracking mulitple objects and bulk support

We can take advantage of the **AddRange()** method:

```
_context.Authors.AddRange(author1,..., authorn)
```

The Remove and Update methods also have a range cunterpart.

The range methods can be used directly in the DbContext.

EF Core supports bulk operations.

# Chapter 06 - Controlling database creation and schema with migrations

## Understanding EF Core migrations

EF Core needs to comprehend:

* How to build queries that work with our db schema (build SQL from our LINQ queries) 
* How do we shape data that's returned from the db (materialize query results into objects)
* How to get that data into the database (build SQL to save data into database)

**Mapping knowledge can also be used to evolve the database schema**.

**EF Core basic migrations workflow**

Define/change model --> Create a migration file --> Apply migration to DB or script

## The design-time migration tools

**Creating and executing migrations happens at design time**.

The NuGet package used for this is **Microsoft.Entity.FrameworkCore.Tools**.

Migration commands led to Migration APIs. There are:

* Powershell migrations commands (**add-migration**)
* dotnet CLI migrations commands (**dotnet ef migrations add**)

If we're going to work with the command line we need to install the EF Core command line tool on our system:

```
dotnet tool install dotnet-ef
```

We also need the **Microsoft.EntityFrameworkCore.Design** NuGet package.

**Visual Studio (Windows)**

* Add tools package to project
	- Design comes for "free"

**Command line**

* Install the tools on your system
* Design package in your project

## Using migrations in Visual Studio when EF Core is in a Class library project

* Install **Microsoft.Entity.FrameworkCore.Tools** NuGet package into executable project (e.g., console)
* Ensure the executable project is the startup project
* Set Package Manager Console (PMC) "default project" to class library with EF Core (e.g., data)
* Run EF Core migration PoerShell commands in PMC

To make sure that we've got it all set up correctly, using the PMC, type the following command:

```
get-help entityframework
```

## Adding a first migration

This is done using the **add-migration** command.

It will look at the DbContext and determine the data model. Using that knowledge, it will create a new migration file describing
how to construct the database schema to match the model.

```
Add-Migration [-Name] <String> [-OutputDir <String>] [-Context <String>] [-Project <String>] [-StartupProject <String>] [-Namespace <String>] [-Args 
<String>] [<CommonParameters>]
```

For instance:

```
add-migration initial 
```

## Inspecting the first migration

It will be created a **Migrations** folder containing a new migration file called **initial** along with a timestamp. There's 
another file in there (in our example, called PubContextModelSnapshot) and that's where EF Core migrations keeps track of the
current state of the model.

This snapshot file is really important because the next time we add a migration, EF Core will start by reading the DbContext to
determine the current state of the data model. Then it will the snapshot of the previous state of the data model and compare it
to the new version of the model, and that's how it figures out what needs to be changed in the schema and is able to create a new
set of migrations to get caught up, and it updates the snapshot file.

## Using migrations to script or directly create the database

We can also generate an script throught the migrations, an importantt feature for working with a production database or sharing
our development database changes with our team.

### Applying migrations directly to the database

The PowerShell command to update the database directly is **update-database**. In response, EF Core:

* Reads migration file
* Generates SQL in memory
* Creates the database if needed
* Runs SQL on the database

### Applying migrations into a SQL script

If we're only creating an script, we'll use the **script-migration++ PowerShell command or dotnet ef migrations script in the CLI.
In this case, EF Core:

* Reads migration file
* Generates SQL
* Default: displays SQL in editor
* Use parameters to target file name, etc.

**Migrations recommendation**

* Development database --> **update-database**
* Production database  --> **script-migration** 

**What if the database doesn't exist?

**update-database**
API's internal code will create the database before executing migration code.

**script-migration**
If we've created a script and we runinng it, we must create the database before running the script.

In the PMC, run the following command first:

```
update-database -verbose
```

## Seeding a database via migrations

Seed data is specified in the modelBuilder with a method called **HasData**.

```
modelBuilder.Entity<EntityType>().HasData(parameters);
modelBuilder.Entit<Author>().HasData(new Author { ...});
```

**Provides all parameters including keys and foreign keys**. 
**HasData will get interpreted into migrations**.
**Inserts will get interpreted into SQL**.
**Data will get inserted when migrations are executed.**

```
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
```

Then, we create a new migration (**add-migration seedauthors**, for instance), fill in the business logic for the both overriden
methods, **Up** and **Down**, and finally from PMC, we run the command **update-database**.

**Seeding with HasData will not cover all use cases for seeding**.

### Use cases for seeding with HasData

* Mostly static seed data.
* Sole means of seeding.
* No dependency on anything else in the database.
* Provide test data with a consistent starting point.

**HasData will also be recognized and applied by EnsureCreated**.

## Scripting multiple migrations

**update-database** know to only apply the latest migration and its logic is to check history table in the database.

Scripting migrations requires more control, so it works differently than **update-database**. Scripts won't check the database.

If we go to use scripting migration, we have to keep in mind what scenarios that make sense for us. 

### Some scripting options

```
script-migration
```
Default: scripts every migration. For example, testing where we might be starting with a fresh database each time.

```
script-migration -idempotent (the smartest option)
```
Scripts all migrations but check for each object first e.g., table already exists.

The script-migration has two parameters:

**FROM**: specifies the last migration run, so start at the next one.
**TO**: final one to apply.

```
script-migration FROM  TO
```

## Reverse engineering an existing database

W've seen the use of migrations to create a database from a DbContext and classes. It's also possible to reverse engineer an existing database into a DbContext and classes.

### Scaffolding builds DbContext and Entity Classes

This s a one-time procedure to get us a head start with our code if we're working with an existing database.

**Scaffolding limitations**

* Updating model when database changes is not currently supported with EF Core 6.

* It's not easy to begin by reverse engineering an existing database and then migrate the database with model changes.

**Reverse Engineer with the Scaffold Command**

The Powershell command to use for this task is **Scaffold-DbContext**. If we're using the EF CLI, it's **dotnet ef dbcontext scaffold**.

Connection and provider parameters are required!!

Run the following command in the PMC:

```
Scaffold-DbContext 'Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = PubDatabase;' Microsoft.EntityFrameworkcore.SqlServer
```

By default, scaffolding prepares our code for "lazy loading" related data.

How EF Core determines mappings to DB?

It has defaults that it follows when it's reading our classes in the DbContext, in order to determine what the database schema looks like. These are referred as **conventions**.

When those conventions don't align with our real intent, in the context file we have the ability to weak how EF Core will interpret the model, and we do that using the **Fluent API**.

Ex.:
```
modelBuilder.Entity<Book>()
    .Property(b => b.Title)
    .HasColumnName("MainTitle");
```

Another path for overriding conventions is using **data annotations**:

```
[Column("MainTitle)]
public string Title { get; set; }
```

# Chapter 06 - Defining one-to-many relationships

## Visualizing EF Core's interpretation of our data model

Install the **EF Core Power Tools** VS extension.

## One-to-many relationships

Reference from parent to child is sufficient. The child has no references back to parent and a foreign key will be inferred in the database.

```
public class Author
{
    ...
    public List<Book> Books { get; set; }
}

public class Book
{
    ...
    public int Id { get; set; }
}
```

## Beneffiting from foreign-key property

In our example, without a foreign key for Author in the Book model, the only way we can add a new book to an author is through the Author object.

```
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public Author Author { get; set; }
}

...

author.Books.Add(abook)
abook.Author = someauthor
```

This means that I always have to have that author in memory in order to add a new book into the system.

However, with a foreign key property available, we don't need an Author object.

```
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int AuthorId { get; set; }
}

...

book.AuthorId = 1
```

**Foreign key properties & HasData seeding**

HasData requires explicit primary and foreign key values to be set. With AuthorId now in Book, we can seed book data as well.

## Mapping unconventional foreign-keys

**EF Core should not drive how we design our business logic**.

### Configuring a non-conventional foreign key

FK is tied to a relationship, so we must first describe that relationship.

**PubContext.cs**

```
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Author>()
        .HasMany(a => a.Books)
        .WithOne(b => b.Author)
        .HasForeignKey(b => b.AuthorFK);
}
```

## Understanding nullability and required vs. optional principals

This is important regarding the one-to-many relationships. That is, requiring that every dependent has a principal or allowing
that to be an optional relationship.

**By default, every dependent must have a principal, but EF Core does not enforce this**.

In our case, that means that every Book must have an Author.

**Allowing optional parent**

```
public class Book
{
    ...
    public int? AuthorId { get; set; }
}
```

We can also use a mapping to specify that the Author isnt' required:

```
modelBuilder.Entity<Author>()
        .HasMany(a => a.Books)
        .WithOne(b => b.Author)
        .HasForeignKey(b => b.AuthorID)
        .IsRequired(false);
```

# CHAPTER 07 - Logging EF Core activity and SQL 

## Adding logging to EF Core's workflow

EF Core's logging is an extension of .NET Logging APIs. 

**EF Core captures**:

* SQL
* ChangeTracker activity
* Interaction with database
* Database transactions

**EF Core specific configurations**:

- EnableDetailedErrors, EnableSensitiveData
- Filter based on message type (e.g., Database messages)
- Even more detailed filtering (see docs)

We can configure it using the DbContextOptionsBuilder.**LogTo** method:

```
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    ...

    optionsBuilder
        .UseSqlServer(configuration.GetConnectionString(CONNECTION_STRING))
        .LogTo(Console.WriteLine);
}
```

There's no default target for where the logs should be sent so, we always have to provide a target as a parameter.

## Filtering log output with LogLevel and Command Type

The unfiltered logs can help us learn what filters can help in different situations.

When configuring the DbContext in the IoC container:

```
services.AddDbContext<PubContext>(options =>
    {        
        options
        .UseSqlServer(configuration.GetRequiredSection("ConnectionStrings").GetSection("PubDatabase").Value)
        .LogTo(Console.WriteLine, new[] {DbLoggerCategory.Database.Command.Name}, LogLevel.Information);        
    },
    ServiceLifetime.Singleton);
```

# CHAPTER 08 - Interacting with related data

## Adding related data

Any object can be the head of a graph. For instance: 

**Author Graph** 
An author with some books in memory.

**Book Graph** 
An book with an author and its book jacket in memory.

**Change Tracker response to new child of existing parent**.

- Add child to child collection of existing tracked parent ==> **SaveChanges**
    * When **SaveChanges** calls **DetectChanges**, the context will see that the book was added to the author.

**DANGEROUS!!**:

Beware accidental inserts! Passing a pre-existing entity into its DbSet.Add, will cause EF Core to try to insert it into the database!

- Add existing tracked parent to ref property of child ==> **SaveChanges**
    * EF Core forced author state to Added when I added via Authors DbSet, but not when adding from Books

- Set foreign key property in child class to parent's key value ==> **Add & SaveChanges** (The favorite one)
    * If I already know the key value of the author, but the object is not yet in-memory, we can just set the AuthorId property of the new book to that key value that we already know.

## Eager loading related data in queries

### Eager loading

Include related objects in query. It allow us to use the DbSet.Include() method to retrieve data and its related data in the same query.
By default, the entire collection is retrieved.

**Using Include for multiple layers of relationships**
```
_dbContext.Authors
.Include(a => a.Books)
.ThenInclude(b => b.BookJackets)
.ToList();
```

* Get books for each author
* Then get the jackets for each book

We can also traverse graphs in multiple directions.

```
_dbContext.Authors
.Include(b => a.Books.BookJackets)
.ToList();
```

* Get books for each author
* Also get the contact info each author

```
_dbContext.Authors
.Include(a => a.Books)
.Include(a => a.ContactInfo)
.ToList();
```

* Get the jackets for each author's book (but don't get the books)

**IMPORTANT!!**
Composing many Includes in one query could create performance issues. Monitor your queries!
Include defaults to a single SQL command. Use **AsSplitQuery()** to send multiple SQL commands instead.

### Query projection

Define the shape of query results.

**Projecting into undefined (anonymous) type**

- Use LINQ's Select method
- Use a lambda expression to specify properties to retrieve
- Instantiate a type to capture the resulting structure
- Anonymous types are not available outside of the method

```
var someType = _dbContext.Authors
    .Select(properties into a new type)
    .ToList();
```

If we're returning more than a single property, then we'll have to contain the new properties within a type: 

```
var someType = _dbContext.Authors
    .Select(a => new { a.FirstName, a.LastName, a.Books.Count() })
    .ToList();
```

EF Core can only track entities recognized by the DbContext:

- Anonymous types **are not tracked**
- Entities that are properties of an anonymous type **are tracked**

### Loading related data for objects in memory

There are two other ways to perform this task:

**Explicit loading**
Explicit request related data for objects in memory.

```
// With author object already in memory, load a collection
_dbContext.Entry(author).Collection(a => a.Books).Load();

// With book object already in memory, load a reference (e.g.: parent or 1:1)
_dbContext.Entry(book).Reference(b => b.Author).Load();
```

### Lazy loading

On-the-fly retrieval of data related to objects in memory.

To enable lazy loading:

1) Every navigation property in every entity must be virtual. Example:
```
public virtual List<Book> Books { get; set; }
```

2) Reference the Microsoft.EntityFramework.Proxies package in the project.

3) Use the proxy logic provided by that package.

```
optionsBuilder.UseLazyLoadingProxies()
```
**Lazy loading**
On-the-fly retrieval of data related to objects in memory.

```
public void LazyLoadingFromAnAuthor()
{
    // requires lazy loading to be set up in the app
    var author = _dbContext.Authors.FirstOrDefault(a => a.LastName.Equals("Howey"));

    foreach (var book in author.Books)
    {
        Console.WriteLine(book.Title);
    }
}
```

**Explicit loading**
Explicit request related data for objects in memory.

```
public void ExplicitLoadCollections()
{
    var author = _dbContext.Authors.FirstOrDefault(a => a.LastName.Equals("Howey"));
    _dbContext.Entry(author).Collection(a => a.Books).Load();
}
```

### Using related data to filter objects

We can take advantage of related data for querying, even if we don't want to retrieve those related objects. For example:

```
public void FilterUsingRelatedData()
{
    var recentAuthors = _dbContext.Authors
        .Where(author => author.Books.Any(book => book.PublishDate.Year >= 2015))
        .ToList();
}
```

Here, we don't need the books, we just want to see the newer authors. So we're able to navigate through the relationship in the Where predicate, and then do a subquery of the author's books.
It's always good to profile the resulting queries in the database, just in case it can be written more efficiently with a procedure or a view. 

### Modifying related data

**EF core's default entity state of graph data**

                        **Has key value**       **No key value**

**Add(graph)**          Added                   Added
**Update(graph)**       Modified                Added
**Attach(graph)**       Unchanged               Added

Objects with no key value, no matter if we use the Add method, the Update or Attach, **will always be marked Added**. But otherwise, if we use the Update method and we pass in a graph, we're telling it to update
the graph.
We do have other ways to start tracking objects and anything that's attached to them. For example, the DbSet Attach method.

**Attach starts tracking with state set to Unchanged**.

So, instead of use the DbSet Add or Update or Remove methods, we can use the **DbContext Entry** method. In EF Core, Entry will focus specially on th entry that we pass in. So for instance, we can pass the Book
object into the Entry method and then use the Entry's State property to set the state of that entry to Modified.

**DbContext,Entry give us a lot of fine-grained control over the change tracker**.

### Understanding deleting within graphs

```
void DeleteAuthor()
{
    var author = _dbContext.Authors.Find(2);
    _dbContext.Authors.Remove(author);
    _dbContext.SaveChanges();
}
```

**Database enforces cascade delete**

Only author is in memory, tracked by EF Core & marked "Deleted". Database's cascade delete will take care of books when author is deleted.

# CHAPTER 09 - Defining and using many-to-many relationships

**New requirements**:
To keep track of the artists who design book covers.
Note that the artists sometimes collaborate on a cover.

EF Core has three ways to implement many-to-many relationships, but the most common and natural of them is refered as **skip navigations**, and 
that means **Direct refs from both ends**.

For more complex needs we have:

* **Skip with payload** : Allows database-generated data in extra columns
* **Explicit join class** : Additional properties accessible via code (create an entity between the two ends that join them)

## Planning the M2M relationship implementation

* Multiple artists working on a cover
* An artist can work on many covers

**Steps**:

* Create new Artist and Cover classes, update PubContext
* Create a migration to reflect the changes needed in the database
* Apply the migration to the database
* Write the code to manage artists and book covers
* Connect books to covers

## Understanding and creating skip navigations

We only need to add properties in those related classes that point to each other.

## Joining objects in new M2M relationships

This means creating varying combinations of two types of objects. For instance:

**Joining Covers and Artists of differing states**

* Existing Cover + Existing Artist: Existing Artist is assigned to a pre-defined book Cover
* New Cover + Existing Artist: New Artist is hired to work on a pre-defined book Cover
* New Cover + New Artist: New Artist is hired and declares a new book Cover

With skip navigations, we have to work with the objects when we're connecting two ends.

**One2M with FK property**
```
public class Book
{
    ... other properties
    public int AuthorId { get, set; }
}
```

**M2M with Skip navigations**
```
public class Artist
{
    ... other properties
    public List<Cover> Covers { get, set; }
}

public class Cover
{
    ... other properties
    public List<Artist> Artists { get, set; }
}
```

## Querying across M2M relationships

**Eager loading**
Include relared objects in the query

**Query projections**
Define the shape of query results

**Explicit loading**
Explicitly request related data for objects in memory

**Lazy loading**
On-the-fly retrieval of data related to objects in memory

## Understanding and benefiting from circular references in graphs

This happens with all relationships, not just M2M.

## Removing joins in M2M relationships

Because we're using skip navigations, the join in the M2M relationship isn't direclty available to us as an object, and that means we will need both ends
of that relationship in memory so we can remove one of the objects from the others list and let the change tracker figure out how to respond to that in
the database.

```
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
```

What if we want to delete an object that is joined to another?

```
void DeleteAnObjectThatIsInARelationship()
{
    var cover = _dbContext.Covers.Find(4);
    _dbContext.Covers.Remove(cover);
    _dbContext.SaveChanges();
}
```

SOLUTION: **Cascade delete to the rescue!!**
If the join is being tracked, EF core will cascade delete the join.
If the relationship is not being tracked, database cascade delete will remove the join.

**DELETING A M2M RELATONSHIPS IS EASIER WITH STORED PROCEDURES!!**

## Changing joins in M2M relationships

For example: reassigning a cover to a different artist.

In One2M, EF core knows the dependent can have only one principal. In M2M, an object can be joined to unlimited partner ends.

### Preferred workflow to changing a join

* Remove the original join
* Then create the new join between the two ends

## Introducing more complex M2M relationships

* **Skips with payoload**: Allows database-generated data in extra columns
* **Explicit join class**: Additional properties accessible via code

# CHAPTER 10 - Defining and using One2One relationships

**DbContext must be able to identify a principal ("parent") and a dependent ("child")**.

### Common ways EF Core identifies One2One

**1) Navigations on both ends with FK independent** => EF core will recognize One2One and identify the dependent.
a) Navigation properties on both ends and a foreign key property in the dependent. 

**2) Navigation on on end. FK on the other
Same as 1a.

**3) Navigation on both ends**
a) EF Core requires a mapping to define principal/dependent

For instance: tying Book and cover together:

Book => It will have a cover property.
Cover => It will have a Book and BookId properties.

**DEFINING ONE2ONE WITH EXISTING DATA CAN CAUSE CONFLICTS WITH DB CONSTRAINTS!!**

**Dependents are optional by default**

**By default, the cover is   optional
- The database constraint will allow books to be inserted without a cover.
- The business logic allows a book's cover to be created at a later date.

**Configure it to be required**
- Map the cover property as IsRequired.
- This causes the database constraint to require a cover.
- EF Core will not enforce the rule; the database will throw an error.
- This is a business rule to be applied in your business logic.

**Prioncipals are required by default**

**Order of operations for the migration**

1) Adds new BookId column
2) Updates the BookId column values
3) Applies Index
4) Adds foreign key constraint

## Querying One2One relationships

Same patterns as for M2M relationships:

**Eager loading**: Include related objects in query.
**Query projections**: Define the shape of query results.
**Explicit loading**: Explicitly request related data for objects in memory.
**Lazy loading**: On-the-fly retrieval of data related to objects in memory.

### Some other queries that can be run:

```
// Get all books with their covers, even if there is no cover
_dbContext.Books
.Include(book => book.Cover)
.ToList();

// Get books that have a cover indeed
_dbContext.Books
.Include(book => book.Cover)
.Where(book => book.Cover != null)
.ToList();

// Get books that does not have a cover yet
_dbContext.Books
.Where(book => book.Cover == null)
.ToList();

// Project an anonymous type of Title and DesignIdeas for books
// who have a cover.
_dbContext.Books
.Where(book => book.Cover != null)
.Select(book => new { b.Title, b.Cover.DesignIdeas })
.ToList();
```

### Multi-level query

Using multiple includes and **ThenInclude** to query more deeply int a graph.

**Performance considerations with Include**

- Composing many Includes in one query could create performance issues. Monitor your queries!!
- Include defaults to a single SQL command. Use **AsSplitQuery** to send multiple SQL commands instead.

## Combining objects on One2One relationships

For instance:
- Add a new book and cover together
- Add cover to existing book that's in memory
- Add cover to existing book that is in DB but not in memory

## Replacing or removing One2One relationships

Use case for this project: Reassigning a Cover to a different Book

* Cover originally assigned to a blue book and reassigning it to a green book

We have several options:

1. We can change the cover's BookId:
```
greenCover.BookId = 3
```

2. If we have the new book in memory, change the navigation property in the dependent object
```
greenCover.Book = greenBookObject
```

3. Change the navigation property of the principal, if we have both books in memory along with the cover.
```
greenCover = blueBook.Cover;
greenBook.Cover = greenCover;
```

If we want to just remove the relationship between a book and an cover, the simplest path is just to delete the cover:
```
_dbContest.Covers.Remove(greenCover); // This will delete the dependent!
```
EF core sends a DELETE to the database for that cover row.

Another simple way is if you have a graph of the book and the cover, and we set the Cover property to null: 
```
bookWithCoverGraph.Cover = null;
```
* ChangeTracker is aware of book and cover.
* EF Core sends a DELETE to the database for that cover row.

**Some factors that affect relationship behavior**

* Is the dependent required?
* Is the child object in memory?
* Are parent or child object being tracked?
* Test the behaviors to learn cause and effect!

# CHAPTER 11 - Working with views and stored procedures and raw SQL

EF Core allow us to work dectly with:
* Raw SQL
* Stored procedures
* Views
* Scalar functions
* MAp to table value functions
* Map to queries in DbContext

## Querying with raw SQL

Used in case we don't want to rely on EF Core to generate the queries for us. There are two methods tat let us to execute raw SQL against our defined DbSets:

**DbSet.FromSqlRaw()**
```
_context.Authors.FromSqlRaw("some sql string").ToList();
```

With it, is important to use parameters to avoid SQL injection.

**DbSet.FromSqlInterpolated()**
```
_context.Authors.FromSqlInterpolated($"some sql string {var}").ToList();
```

## Keeping the database safe with parameterized raw SQL queries

**NEVER USE SQL WITH PARAMETERS EMBEDDED DIRECTLY INTO THE STRING!!**

* FromSqlInterpolated expects one formatted string as its parameter
* FromSqlInterpolated will not accept a string

## Adding stored procedures and other database objects using migrations

EF core's raw SQL methods support calling stored procedures and return entities. As we need a stored procedure in the dtabase, we will use EF Core migrations to do the job.
It's better to keep raw SQL out of the code, and instead, store it in the database in the forms of stored procedures, views and other database objects.

**Example of stored procedure for this project**:

Retrieve authors who published a book in a range of years.
```
EXEC thesproc startyear, endyear
```

### Workflow for adding db objects with migrations

1. Work out the query directly against the database
2. Build the command to create the procedure
```
CREATE PROCEDURE dbo.AuthorsPublishedInYearRange
	@yearstart int,
	@yearend int
AS
SELECT DISTINCT Authors.* FROM authors
LEFT JOIN Books ON Authors.AuthorId=books.authorId
WHERE Year(books.PublishDate)>=@yearstart AND Year(books.PublishDate)<=@yearend
```

3. Test the command by creating & running the stored procedure
4. Remove the procedure from the database

But to create the stored procedure, we will use the migration workflow, because doing it like this, it will be part of the shared source code.

* Other tram members can easily migrate their own development databases.
* The migration becomes part of our source code.
* Useful in other environments such as CI/CD, acceptance testing and possibly production.

5. Add the create procedure command to a new migration
```
add-migration addstoredproc
```

This will create a new migration file with an empty Up and an empty Down methods (they're empty because it didnt't discover any changes in the data model).
The migrations API has a method called **Sql** that let us pass in raw SQL for it to run against the database. We will paste the code used to create the stored procedure manually, within the **Up** method:
```
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(
        @"CREATE PROCEDURE dbo.AuthorsPublishedInYearRange
        @yearstart int,@yearend int
        AS
        SELECT DISTINCT Authors.*
        FROM authors
        LEFT JOIN Books ON Authors.AuthorId=books.authorId
        WHERE Year(books.PublishDate)>=@yearstart AND Year(books.PublishDate)<=@yearend
        ");
}
```

**One of the string literal (@) benefits is allowing multi-line strings**

The **Down** method is used if we need to revert the migration:
```
protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(@"DROP PROCEDURE dbo.AuthorsPublishedInYearRange");
}
```

Now, we can go ahead and update the database with this new migration:
```
update-database
```

## Running stored procedure queries with raw SQL

**Querying via DbSets using stored procedures** (using FromSqlRaw with formatted string)
```
DbSet.FromRawSql("Exec MyStoredProc, {0}, {1}", firstValue, secondValue)
```

or using **FromSqlInterpolated with interpolated string**:
```
DbSet.FromSql($"Exec MyStoredProc {firstValue}, {secondValue}")
```

### Examples of composing on raw SQL with Sprocs

**Other methods like OrderBy or even AsNoTracking**
```
_dbContext.Authors
.FromSqlRaw("AuthorsSproc, {0}, {1}", 2010, 2015)
.OrderBy(a => a.LastName)
.ToList();
```

**Different LINQ execution methods**
```
_dbContext.Authors
.FromSqlRaw("AuthorsSproc, {0}, {1}", 2010, 2015)
.Include(a => a.Books)
.FirstOrDefault();
```

**Eager load with Include**
```
_dbContext.Authors
.FromSqlRaw("AuthorsSproc, {0}, {1}", 2010, 2015)
.Include(a => a.Books)
.ToList();
```

## Using keyless entities to map to Views

Historically, EF and EF Core only understood entites with keys, because tracker relies on those key properties, but noe EF Core can comprehend keless entities.

**Keyless entities != Non-tracking queries**

Non-tracking query

* Entity has a key prop
* No-tracking is optional
* Maps to tables with PK

Mixed use

* Entity has a key prop
* MAps to vew and table
* Query from the view, update to the table

**KEYLESS ENTITIES ARE ALWAYS READ-ONLY AND WILL NEVER BE TRACKED. FULL STOP.**

```
...

public DbSet<AuthorByArtist> AuthorsByArtist { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    if (modelBuilder is null)
    {
        throw new ArgumentNullException(nameof(modelBuilder));
    }

    modelBuilder.Entity<AuthorByArtist>()
        .HasNoKey()
        .ToView(nameof(AuthorsByArtist));

...
```

Migrations will not attempt to create a database view that's mapped in a DbContext.

## Querying the database views

Not all DbSet methods work with keyless entities: Find() will compile, but it will fail at runtime!

## Executing non-query raw SQL commands

It's possible to send raw SQL to the database for other tasks using the **ExecuteSqlRaw** and **ExecuteSqlInterpolated** methods. These are used to execute raw SQL and stored procs.
They are not DbSet methods.

**Run raw SQL for non-query commands from the Database property**
```
_dbContext.Database.ExecuteSQLRaw("some SQL string");

_dbContext.Database.ExecuteSQLRawAsync("some SQL string");

_dbContext.Database.ExecuteSQLInterpolated($"some SQL string {variable}");

_dbContext.Database.ExecuteSQLInterpolatedAsync($"some SQL string {variable}");
```

The only result returned by those commands is the number of rows affected.
We can pass in SQL or call procedures with ExecuteSQL methods too. Here, an example of migration to create an stored procedure to delete records from the Covers table in the database:
```
public partial class AddDeleteSproc : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            @"CREATE PROCEDURE DeleteCover
                @coverId int
              AS
              DELETE FROM covers WHERE CoverId = @coverId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE DeleteCover");
    }
}
```

Let's execute this stored procedure:
```
public void DeleteCover(int coverId)
{
    var rowCount = _dbContext.Database.ExecuteSqlRaw("DeleteCover {0}", coverId);
    Console.WriteLine(rowCount);
}
```

# CHAPTER 12 - Using EF Core with ASP .NET Core Apps

## Reviewing EF Core's lifecycle in disconnected apps

The lifecycle of a DbContext is different in web applications because of their disconnected nature.

**Working in a single DbContext instance**

Retrieve data                   Modify objects      Save changes
     |                                |
Context starts tracking                             Context updates state
state of each returned object                       of tracked objects
                                                    before determining SQL

**Various was to inform Context of state**

* DbSet methods (Add, Update, Remove)

* Set DbEntry.State => EntityState.Deleted/Detached/Modified/...
    - Setting the entry state explicitly through the context.

* Retrieve and modify from database.
    - Pulling down the current values from the database and applying the incoming values to those objects directly.
    ```
    public void UpdateDbAuthorValues(Author aFromRequest)
    {
        var a = _dbContext.Authors
                .Find(aFromRequest.AuthorId);
        // set values with aFromRequestValues
    }
    ```

**IN DISCONNECTED SCENARIOS, IT'S UP TO YO TO INFORM THE CONTEXT ABOUT OBJECT STATE**.