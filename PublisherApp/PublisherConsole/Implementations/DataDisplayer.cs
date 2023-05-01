using PublisherConsole.Interfaces;

namespace PublisherConsole.Implementations;

public class DataDisplayer : IDataDisplayer
{
    public void DisplayData<T>(T item)
    {
        Console.WriteLine(item);
    }
}
