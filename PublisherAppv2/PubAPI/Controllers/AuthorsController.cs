using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubAPI.Dtos;
using PubAPI.Interfaces;
using PublisherData;
using PublisherDomain;

namespace PubAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorsController : ControllerBase
{
    private readonly PubContext _context;
    private readonly IMappingService _mappingService;

    public AuthorsController(
        PubContext context,
        IMappingService mappingService)
    {
        _context = context;
        _mappingService = mappingService;
    }

    // GET: api/Authors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
    {
        if (_context.Authors == null)
        {
            return NotFound();
        }

        var result = await _context.Authors.ToListAsync();

        return Ok(_mappingService.MapEntityListToDtoList<Author, AuthorDto>(result));
    }

    // GET: api/Authors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
    {
        if (_context.Authors == null)
        {
            return NotFound();
        }

        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.AuthorId == id);

        if (author == null)
        {
            return NotFound();
        }

        return Ok(_mappingService.MapEntityToDto<Author, AuthorDto>(author));
    }

    // PUT: api/Authors/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAuthor(int id, AuthorDto authorDto)
    {
        if (id != authorDto.AuthorId)
        {
            return BadRequest();
        }

        var author = AuthorFromDto(authorDto);
        _context.Entry(author).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AuthorExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Authors
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<AuthorDto>> PostAuthor(AuthorDto authorDto)
    {
        if (_context.Authors == null)
        {
            return Problem("Entity set 'PubContext.Authors'  is null.");
        }

        var author = AuthorFromDto(authorDto);
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetAuthor", new { id = author.AuthorId }, AuthorToDto(author));
    }

    // DELETE: api/Authors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        if (_context.Authors == null)
        {
            return NotFound();
        }

        //var author = await _context.Authors.FindAsync(id);

        //if (author == null)
        //{
        //    return NotFound();
        //}

        //_context.Authors.Remove(author);
        //await _context.SaveChangesAsync();

        var recount = await _context.Database
            .ExecuteSqlInterpolatedAsync($"Delete from authors where authorid = {id}");

        if (recount == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    private bool AuthorExists(int id)
    {
        return (_context.Authors?.Any(e => e.AuthorId == id)).GetValueOrDefault();
    }
    private static ActionResult<AuthorDto> AuthorToDto(Author author)
    {
        return new AuthorDto
        {
            AuthorId = author.AuthorId,
            FirstName = author.FirstName,
            LastName = author.LastName
        };
    }

    private static Author AuthorFromDto(AuthorDto authorDto)
    {
        if (authorDto is null)
        {
            throw new ArgumentNullException(nameof(authorDto));
        }

        return new Author
        {
            AuthorId = authorDto.AuthorId,
            FirstName = authorDto?.FirstName,
            LastName = authorDto?.LastName
        };
    }
}
