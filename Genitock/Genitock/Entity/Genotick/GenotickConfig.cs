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
        /// start date for genotick
        /// </summary>
        public DateTime StartingPoint { get; set; }
        /// <summary>
        /// end date for genotick
        /// </summary>
        public DateTime EndingPoint { get; set; }

        /// <summary>
        /// All currencies data file
        /// </summary>
        public List<String> CurrenciesDataFile;
        /// <summary>
        /// All currencies reverse data file
        /// </summary>
        public List<String> ReverseCurrenciesDataFile;


        /// <summary>
        /// store the confnig file path
        /// </summary>
        private String ConfigPath;

        /// <summary>
        /// genotick binary path
        /// </summary>
        private String _GenotikPath;

        /// <summary>
        /// all config line
        /// </summary>
        private List<String> configContent;


       
        public GenotickConfig(String Genotickpath, String configFileName)
        {
            String configfilepath = Path.Combine(Genotickpath, configFileName);
            ConfigPath = configfilepath;
            _GenotikPath = Genotickpath;
            configContent = File.ReadAllLines(configfilepath).ToList();
            DataDirectory = Path.Combine(Genotickpath, configContent.First(p => p.StartsWith("dataDirectory")).Substring(13).Trim());

            //store all normal file in the same variable and all reverse in another one
            //all file which are not started with reverse are the original file
            CurrenciesDataFile = Directory.GetFiles(DataDirectory).ToList().Where(p => !p.Contains("reverse")).ToList();

            //all files which are reversed one
            ReverseCurrenciesDataFile = Directory.GetFiles(DataDirectory).ToList().Where(p => p.Contains("reverse")).ToList();
            
            //dummy must be computed based on the cvs file
            //StartingPoint = DateTime.ParseExact(configContent.First(p => p.StartsWith("startTimePoint")).Substring(15).Trim(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            //EndingPoint = DateTime.ParseExact(configContent.First(p => p.StartsWith("endTimePoint")).Substring(12).Trim(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        }

        ///Sauvegarde les nouvelles dates d'analyse
        public void SaveConfig()
        {
            String configfilepath = Path.Combine(_GenotikPath, ConfigPath);
            //update date interval
            configContent.Where(p => p.StartsWith("startTimePoint")).ToList().ForEach(s => s = String.Concat("startTimePoint " + StartingPoint.ToString("yyyyMMddHHmmss")));
            configContent.Where(p => p.StartsWith("endTimePoint")).ToList().ForEach(s => s = String.Concat("endTimePoint " + StartingPoint.ToString("yyyyMMddHHmmss")));

        }
    }
}
