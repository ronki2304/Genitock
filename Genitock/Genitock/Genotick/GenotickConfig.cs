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

namespace Genitock.Genotick
{
    public static class GenotickConfig
    {
        /// <summary>
        /// contain the absolute data directory name
        /// </summary>
        public static String FullDataDirectory { get; private set; }

        /// <summary>
        /// contain only the the name of the data directory
        /// </summary>
        public static String DataDirectory { get; private set; }

        public static String DataBackupDirectory
        {
            get
            {
                //todo parametrize this folder
                return Path.Combine(FullDataDirectory, "Backup");
            }
        }
        /// <summary>
        /// max date already present in the CSV
        /// </summary>
        static DateTime PreviousEndingPoint { get; set; }

        /// <summary>
        /// Start date for the next run of genitock
        /// </summary>
        public static DateTime StartingPoint
        {
            get
            {
                return PreviousEndingPoint.AddSeconds((Int32)PoloniexPeriod);
            }
        }
        /// <summary>
        /// end date for the next genotick run
        /// </summary>
        public static DateTime NextEndingPoint
        {
            get
            {
                //calculate the last intervalle available for genotick
                DateTime computedate= StartingPoint.AddSeconds((Int32)PoloniexPeriod);
                while (computedate <= DateTime.UtcNow)
                {
                    computedate =computedate.AddSeconds((Int32)PoloniexPeriod);
                }
                return computedate.AddSeconds((Int32)PoloniexPeriod * -1);
            }
        }

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
        internal static String ConfigPath;

        /// <summary>
        /// genotick binary path
        /// </summary>
        internal static String _GenotikPath;

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
            DataDirectory = configContent.First(p => p.StartsWith("dataDirectory")).Substring(13).Trim();
            FullDataDirectory = Path.Combine(Genotickpath,DataDirectory);

            if (!Directory.Exists(DataBackupDirectory))
                Directory.CreateDirectory(DataBackupDirectory);

            //store all normal file in the same variable and all reverse in another one in a dictionnary with the par as key
            //all file which are not started with reverse are the original file
            foreach (var p in (Directory.GetFiles(FullDataDirectory).ToList().Where(p => !p.Contains("reverse"))))
            {
                CurrenciesDataFileName.Add((Pair)Enum.Parse(typeof(Pair), p.Substring(0, p.IndexOf('.')).Substring(p.LastIndexOf('\\') + 1)));
            }

            //compute the new starting point

            String line = File.ReadLines(FileHelper.getFullFileName(CurrenciesDataFileName.First(), false)).Last();
            PreviousEndingPoint = DateTime.ParseExact(line.Split(',').First(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        }



        ///Sauvegarde les nouvelles dates d'analyse
        public static void SaveConfig()
        {
            String configfilepath = Path.Combine(_GenotikPath, ConfigPath);
            //update date interval
            for (int i =0;i<configContent.Count;i++)
            {
                if (configContent[i].StartsWith("startTimePoint"))
                {
                    configContent[i] = String.Concat("startTimePoint " + StartingPoint.ToString("yyyyMMddHHmmss"));
                }
                if (configContent[i].StartsWith("endTimePoint"))
                {
                    configContent[i] = String.Concat("endTimePoint " + NextEndingPoint.ToString("yyyyMMddHHmmss"));
                }
            }
            File.WriteAllLines(configfilepath, configContent);
        }
    }
}
