using PublisherConsole.Dtos;
using PublisherData;

namespace PublisherConsole;

public class DataLogic
{
    PubContext _context;

    public DataLogic()
    {
        _context = new PubContext();
    }
    public DataLogic(PubContext context)
    {
        _context = context;
    }

    public int ImportAuthors(List<ImportAuthorDto> authorList)
    {
        foreach (var author in authorList)
        {
            _context.Authors.Add(
                new PublisherDomain.Author { FirstName = author.FirstName, LastName = author.LastName }
                );
        }

        return _context.SaveChanges();
    }
}
