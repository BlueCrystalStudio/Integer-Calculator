using Domain;
using System.Xml.Serialization;

namespace Infrastructure.Configuration;
public class ConfigurationManager
{
    public const string ConfigurationFileName = "IntegerCalculatorSettings.xml";
    public Configuration Configuration { get; private set; }
    IXMLHandler xmlHandler;

    public ConfigurationManager(IXMLHandler xmlHandler)
    {
        this.xmlHandler = xmlHandler;
        Load();
    }

    public void ChangeLastEvaluatedFile(string evaluatedFilePath)
    {
        if (Configuration.LastEvaluatedFile == evaluatedFilePath)
            return;

        Configuration.LastEvaluatedFile = evaluatedFilePath;
        Save();
    }

    public void ChangeOutputFolderPath(string outputFolderPath)
    {
        if (Configuration.OutputPath == outputFolderPath)
            return;

        Configuration.OutputPath = outputFolderPath;
        Save();
    }

    public void ChangeOutputExportOptionPath(bool isEnabled)
    {
        if(Configuration.IsOutputToFolderEnabled == isEnabled)
            return;

        Configuration.IsOutputToFolderEnabled = isEnabled;
        Save();
    }

    private void Save()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
        var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationFileName);

        xmlHandler.Save(Configuration, path);
    }

    private void Load()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationFileName);

        try
        {
            Configuration = xmlHandler.Load<Configuration>(path)!;   // This throws FileNotFoundException on every first run but I do not want to bother user with it. Could be logged somewhere that Config was initialized.
        }
        catch
        {
            Configuration = new ();
        }
    }
}
