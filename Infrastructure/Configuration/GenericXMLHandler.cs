using Domain;
using System.Xml.Serialization;

namespace Infrastructure.Configuration;
public class GenericXMLHandler : IXMLHandler
{
    public T? Load<T>(string filePath) where T : class
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file at path {filePath} was not found.");
        }

        XmlSerializer serializer = new(typeof(T));
        using (var reader = new StreamReader(filePath))
        {
            return serializer.Deserialize(reader) as T;
        }
    }

    public void Save<T>(T obj, string filePath) where T : class
    {
        XmlSerializer serializer = new(typeof(T));
        using (var writer = new StreamWriter(filePath, false))
        {
            serializer.Serialize(writer, obj);
        }
    }
}
