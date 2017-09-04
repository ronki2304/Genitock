using Genitock.Entity.Poloniex;
using Genitock.Utility;
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
        /// max date present in the CSV
        /// </summary>
        public static DateTime LastEndingPoint { get; set; }

        /// <summary>
        /// Start date for the next run of genitock
        /// </summary>
        public static DateTime StartingPoint
        {
            get
            {
                return LastEndingPoint.AddSeconds((Int32)PoloniexPeriod);
            }
        }
        /// <summary>
        /// end date for genotick
        /// </summary>
        public static DateTime EndingPoint { get; set; }

        /// <summary>
        /// All currencies data file name
        /// </summary>
        public static List<Pair> CurrenciesDataFileName;

        /// <summary>
        /// the candle period used during all processes
        /// </summary>
        public static Period PoloniexPeriod { get; private set; }


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
            CurrenciesDataFileName = new List<Pair>();
            String Genotickpath = ConfigurationManager.AppSettings["genotick_Path"];
            String configFileName = ConfigurationManager.AppSettings["genotick_configfileName"];
            PoloniexPeriod = (Period)Convert.ToInt32(ConfigurationManager.AppSettings["Poloniex_Candle_Period"]);
            String configfilepath = Path.Combine(Genotickpath, configFileName);
            ConfigPath = configfilepath;
            _GenotikPath = Genotickpath;
            configContent = File.ReadAllLines(configfilepath).ToList();
            DataDirectory = Path.Combine(Genotickpath, configContent.First(p => p.StartsWith("dataDirectory")).Substring(13).Trim());

            //store all normal file in the same variable and all reverse in another one in a dictionnary with the par as key
            //all file which are not started with reverse are the original file
            foreach (var p in (Directory.GetFiles(DataDirectory).ToList().Where(p => !p.Contains("reverse"))))
            {
                CurrenciesDataFileName.Add((Pair)Enum.Parse(typeof(Pair), p.Substring(0, p.IndexOf('.')).Substring(p.LastIndexOf('\\') + 1)));
            }

            //compute the new starting point
            
            String line = File.ReadLines(FileHelper.getFullFileName(CurrenciesDataFileName.First(),false)).Last();
            LastEndingPoint = DateTime.ParseExact(line.Split(',').First(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
          
        }

       

        ///Sauvegarde les nouvelles dates d'analyse
        public static void SaveConfig()
        {
            String configfilepath = Path.Combine(_GenotikPath, ConfigPath);
            //update date interval
            configContent.Where(p => p.StartsWith("startTimePoint")).ToList().ForEach(s => s = String.Concat("startTimePoint " + LastEndingPoint.ToString("yyyyMMddHHmmss")));
            configContent.Where(p => p.StartsWith("endTimePoint")).ToList().ForEach(s => s = String.Concat("endTimePoint " + LastEndingPoint.ToString("yyyyMMddHHmmss")));

        }
    }
}
