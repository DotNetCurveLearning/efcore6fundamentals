namespace PublisherConsole.Dtos;

public class ImportAuthorDto
{
    private string _firstName;
    private string _lastName;

    public ImportAuthorDto(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
    }

    public string FirstName => _firstName;
    public string LastName => _lastName;
}
