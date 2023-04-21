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

## Filtering queries securely by defaault

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


