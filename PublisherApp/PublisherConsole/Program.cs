
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PublisherConsole;
using PublisherConsole.Aspects;
using PublisherConsole.Implementations;
using PublisherConsole.Interfaces;
using PublisherData;
using PublisherDomain;

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

                // add services
                services.AddDbContext<PubContext>(options =>
                {
                    options
                    .UseSqlServer(configuration.GetRequiredSection("ConnectionStrings").GetSection("PubDatabase").Value)
                    .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                    .EnableSensitiveDataLogging();
                },
                ServiceLifetime.Singleton);

                services.AddTransient<EFCoreDemo>();
                services.AddTransient<CustomLogAttribute>();
                services.AddTransient<IDataDisplayer, DataDisplayer>();
            });

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
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    // add services
    services.AddDbContext<PubContext>(options =>
    {
        options
        .UseSqlServer(configuration.GetRequiredSection("ConnectionStrings").GetSection("PubDatabase").Value)
        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
        .EnableSensitiveDataLogging();
    },
    ServiceLifetime.Singleton);

    services.AddTransient<EFCoreDemo>();
    services.AddTransient<CustomLogAttribute>();
    services.AddTransient<IDataDisplayer, DataDisplayer>();
}

CreateHostBuilder(args).Build().Run();


