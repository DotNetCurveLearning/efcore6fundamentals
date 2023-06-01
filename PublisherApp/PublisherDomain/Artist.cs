using System.Text;

namespace PublisherDomain;

public class Artist
{
    public int ArtistId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ICollection<Cover> Covers { get; set; }

    public Artist() => Covers = new List<Cover>();

    public override string ToString()
    {
        return new StringBuilder()
            .Append(LastName)
            .Append(" ")
            .Append(FirstName)
            .ToString();
    }
}
