namespace PublisherDomain;

public class Cover
{
    public int CoverID { get; set; }
    public string DesignIdeas { get; set; }
    public bool DigitalOnly { get; set; }
    public List<Artist> Artists { get; set; }
}