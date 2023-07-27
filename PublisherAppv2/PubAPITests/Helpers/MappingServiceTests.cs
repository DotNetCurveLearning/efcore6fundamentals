using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PubAPI.Dtos;
using PubAPI.Interfaces;
using PublisherData;
using PublisherDomain;

namespace PubAPI.Helpers.Tests;

[TestClass()]
public class MappingServiceTests
{
    private IMappingService _mappingService;

    [TestInitialize]
    public void Initialize()
    {
        var configurationProvider = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();

            // Reverse mapping configuration from AuthorDto to Author
            cfg.CreateMap<AuthorDto, Author>();
        });

        _mappingService = new MappingService(configurationProvider.CreateMapper());
    }

    [TestMethod()]
    public void MapEntityToDto_ShouldMapCorrectly()
    {
        // Arrange
        var authorModel = new Author { FirstName = "a", LastName = "b" };

        // Act
        var authorDto = _mappingService.MapEntityToDto<Author, AuthorDto>(authorModel);

        // Assert
        Assert.AreEqual(authorModel.AuthorId, authorDto.AuthorId);
        Assert.AreEqual(authorModel.FirstName, authorDto.FirstName);
    }

    [TestMethod()]
    public void MapDtoToEntity_ShouldMapCorrectly()
    {
        // Arrange
        var authorDto = new AuthorDto { AuthorId = 1, FirstName = "a", LastName = "b" };

        // Act
        var authorModel = _mappingService.MapEntityToDto<AuthorDto, Author>(authorDto);

        // Assert
        Assert.AreEqual(authorModel.AuthorId, authorDto.AuthorId);
        Assert.AreEqual(authorModel.FirstName, authorDto.FirstName);
    }
}