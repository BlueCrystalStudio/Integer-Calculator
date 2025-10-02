using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configuration;

[Serializable]
public class Configuration
{
    public bool IsOutputToFolderEnabled = true;
    public string OutputPath = string.Empty;
    public string LastEvaluatedFile = "None";
}
