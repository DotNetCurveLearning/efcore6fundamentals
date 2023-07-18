namespace PubAPI.Dtos;

public class AuthorWithBooksDto
{
    public AuthorWithBooksDto()
    {
        Books = new List<BookDto>();
    }

    public int AuthorId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public List<BookDto> Books { get; set; }
}
