using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubAPI.Dtos;
using PublisherData;
using PublisherDomain;

namespace PubAPI.Helpers;

public class DataLogic
{
    private PubContext _context;

    public DataLogic(PubContext context)
    {
        _context = context;
    }

    public async Task<List<AuthorDto>> GetAllAuthors()
    {
        var authorList = await _context.Authors.ToListAsync();
        List<AuthorDto> authorDtoList = new();

        foreach (var author in authorList)
        {
            authorDtoList.Add(AuthorToDto(author));
        }

        return authorDtoList;
    }

    private static AuthorDto AuthorToDto(Author author)
    {
        return new AuthorDto
        {
            AuthorId = author.AuthorId,
            FirstName = author.FirstName,
            LastName = author.LastName
        };
    }
}
