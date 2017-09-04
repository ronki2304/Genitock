using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Entity.Genotick
{
    public static class GenotickConfig
    {
        /// <summary>
        /// Folder name which contains the ticker data
        /// </summary>
        public static String DataDirectory { get; private set; }
        /// <summary>
        /// start date for genotick
        /// </summary>
        public static DateTime StartingPoint { get; set; }
        /// <summary>
        /// end date for genotick
        /// </summary>
        public static DateTime EndingPoint { get; set; }

        /// <summary>
        /// All currencies data file
        /// </summary>
        public static List<String> CurrenciesDataFile;
        /// <summary>
        /// All currencies reverse data file
        /// </summary>
        public static List<String> ReverseCurrenciesDataFile;


        /// <summary>
        /// store the confnig file path
        /// </summary>
        private static String ConfigPath;

        /// <summary>
        /// genotick binary path
        /// </summary>
        private static String _GenotikPath;

        /// <summary>
        /// all config line
        /// </summary>
        private static List<String> configContent;


       
        static GenotickConfig()
        {
            String Genotickpath = ConfigurationManager.AppSettings["genotick_Path"];
            String configFileName = ConfigurationManager.AppSettings["genotick_configfileName"];
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
        public static void SaveConfig()
        {
            String configfilepath = Path.Combine(_GenotikPath, ConfigPath);
            //update date interval
            configContent.Where(p => p.StartsWith("startTimePoint")).ToList().ForEach(s => s = String.Concat("startTimePoint " + StartingPoint.ToString("yyyyMMddHHmmss")));
            configContent.Where(p => p.StartsWith("endTimePoint")).ToList().ForEach(s => s = String.Concat("endTimePoint " + StartingPoint.ToString("yyyyMMddHHmmss")));

        }
    }
}
