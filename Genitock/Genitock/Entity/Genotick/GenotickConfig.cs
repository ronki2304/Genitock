using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Entity.Genotick
{
    public class GenotickConfig
    {
        /// <summary>
        /// Folder name which contains the ticker data
        /// </summary>
        public String DataDirectory { get; private set; }

        /// <summary>
        /// store the confnig file path
        /// </summary>
        private String ConfigPath;

        /// <summary>
        /// genotick binary path
        /// </summary>
        private String _GenotikPath;


        public DateTime StartingPoint { get; set; }
        public DateTime EndingPoint { get; set; }
        public GenotickConfig(String Genotickpath, String configFileName)
        {
            String configfilepath = Path.Combine(Genotickpath, configFileName);
            ConfigPath = configfilepath;
            _GenotikPath = Genotickpath;
            List<String> configContent = File.ReadAllLines(configfilepath).ToList();
            DataDirectory = Path.Combine(Genotickpath,configContent.First(p => p.StartsWith("dataDirectory")).Substring(13).Trim());

            StartingPoint = DateTime.ParseExact(configContent.First(p => p.StartsWith("startTimePoint")).Substring(15).Trim(),"yyyyMMddHHmmss",CultureInfo.InvariantCulture);
            EndingPoint= DateTime.ParseExact(configContent.First(p => p.StartsWith("endTimePoint")).Substring(12).Trim(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        }

        ///Sauvegarde les nouvelles dates d'analyse
        public void SaveConfig()
        {

        }
    }
}
