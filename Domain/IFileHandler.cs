namespace Domain;

public interface IFileHandler
{
    /// <summary>
    /// Checks and Load specified file on <paramref name="path"/> and return all read lines as string array.
    /// </summary>
    /// <param name="path">Path to file</param>
    /// <returns>Read Lines</returns>
    /// <exception cref="FileNotFoundException"></exception>
    Task<string[]> Load(string path);
    Task Save(string path, string[] lines);
}