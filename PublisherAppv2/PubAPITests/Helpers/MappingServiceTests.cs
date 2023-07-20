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
    public void MapEntityToDtoTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    public void MapEntityListToDtoListTest()
    {
        Assert.Fail();
    }
}