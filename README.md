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