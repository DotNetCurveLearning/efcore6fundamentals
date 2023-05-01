
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PublisherConsole;
using PublisherConsole.Aspects;
using PublisherConsole.Implementations;
using PublisherConsole.Interfaces;
using PublisherData;

static void ConfigureServices(IServiceCollection services)
{
    // configure logging
    //services.AddLogging(builder =>
    //{
    //    builder.AddConsole();
    //    builder.SetMinimumLevel(LogLevel.Information);
    //});

    services.AddOptions();

    // build config
    IConfiguration configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    // add services
    services.AddDbContext<PubContext>(options =>
    {        
        options
        .UseSqlServer(configuration.GetRequiredSection("ConnectionStrings").GetSection("PubDatabase").Value)
        .LogTo(Console.WriteLine, new[] {DbLoggerCategory.Database.Command.Name}, LogLevel.Information);        
    },
    ServiceLifetime.Singleton);

    services.AddTransient<EFCoreDemo>();
    services.AddTransient<CustomLogAttribute>();
    services.AddTransient<IDataDisplayer, DataDisplayer>();
}

// create service collection
var services = new ServiceCollection();
ConfigureServices(services);  

// create service provider
using var serviceProvider = services.BuildServiceProvider();

var efCoreDemo = serviceProvider.GetService<EFCoreDemo>();

//efCoreDemo?.DisplayData(efCoreDemo?.FindThatAuthor(1));
//efCoreDemo.SortAuthors();
//efCoreDemo?.SkipAndTakeAuthors();
efCoreDemo?.GetAuthorsWithBooks();