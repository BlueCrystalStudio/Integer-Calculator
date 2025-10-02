namespace Domain;

public interface IXMLHandler
{
    public T? Load<T>(string filePath) where T : class;
    public void Save<T>(T obj, string filePath) where T : class;
}