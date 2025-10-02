using Domain;

namespace Infrastructure;
public class FileHandler : IFileHandler
{
    // InheritDoc
    public string[] Load(string path)
    {
        if (File.Exists(path) == false)
        {
            throw new FileNotFoundException("The specified file was not found.", path);
        }

        return File.ReadAllLines(path);
    }

    public async Task Save(string path, string[] lines) => await File.AppendAllLinesAsync(path, lines);
}
