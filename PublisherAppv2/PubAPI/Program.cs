using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PubAPI.Dtos;
using PubAPI.Helpers;
using PubAPI.Interfaces;
using PublisherDomain;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// For the data fetched, it will ignore all the cyclical data and
// just send back the core of the results.
builder.Services.AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PublisherData.PubContext>(
    opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PubConnection"))
              .EnableSensitiveDataLogging()
              .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

//builder.Services.AddSingleton<DataLogic>();
builder.Services.AddTransient<IMappingService, MappingService>();

builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

// Generic method to register mappings
static void RegisterMappings<TSource, TDestination>(IMapperConfigurationExpression cfg)
{
    cfg.CreateMap<TSource, TDestination>(); 
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
