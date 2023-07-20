using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PublisherConsole;
using PublisherConsole.Dtos;
using PublisherConsole.Implementations;
using PublisherConsole.Interfaces;
using PublisherData;

static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddOptions();

                // build config
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                string connectionString = configuration.GetRequiredSection("ConnectionStrings").GetSection("PubDatabase").Value ??
                                          configuration.GetRequiredSection("ConnectionStrings").GetSection("DefaultConnection").Value;

                // add services
                services.AddDbContext<PubContext>(options =>
                {
                    if (!options.IsConfigured)
                    {
                        options.UseSqlServer(connectionString)
                        .LogTo(Console.WriteLine, 
                        new[] { DbLoggerCategory.Database.Command.Name }, 
                        LogLevel.Information)
                        .EnableSensitiveDataLogging();
                    }
                },
                ServiceLifetime.Singleton);

                services.AddTransient<EFCoreDemo>();
                services.AddTransient<IDataDisplayer, DataDisplayer>();
                services.AddSingleton<DataLogic>();
            });

var host = CreateHostBuilder(args).Build();
var efCoreDemo = host.Services.GetRequiredService<EFCoreDemo>();
// efCoreDemo.DeleteCover(8);
var dataLogic = host.Services.GetRequiredService<DataLogic>();


AddSomeAuthors();

void AddSomeAuthors()
{
    var authorList = new List<ImportAuthorDto>()
            {
                new ImportAuthorDto("Ruth", "Ozeki"),
                new ImportAuthorDto("Sofia", "Segovia"),
                new ImportAuthorDto("Ursula K.", "LeGuin"),
                new ImportAuthorDto("Hugh", "Howey"),
                new ImportAuthorDto("Isabelle", "Allende"),
            };

    var result = dataLogic.ImportAuthors(authorList);
}