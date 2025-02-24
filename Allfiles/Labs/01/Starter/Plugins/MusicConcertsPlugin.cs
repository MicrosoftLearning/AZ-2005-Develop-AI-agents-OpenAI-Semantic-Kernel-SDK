using Microsoft.SemanticKernel;

public class MusicConcertsPlugin
{
    [KernelFunction("GetConcerts")]
    public static string GetConcerts()
    {
        string content = File.ReadAllText($"Files/ConcertDates.txt");
        return content;
    }
}