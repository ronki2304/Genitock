using Genitock.Entity;
using Genitock.Entity.Genotick;
using Genitock.Entity.Poloniex;
using Genitock.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Genotick
{
    public class InputData
    {
        #region write new file
        public static void WriteToCSVFile(Chart data, String outputDirectory)
        {
            String filename = Path.Combine(outputDirectory, String.Concat(data.pair.ToString(), ".csv"));
            String reversefilename = Path.Combine(outputDirectory, String.Concat("reverse_", data.pair.ToString(), ".csv"));

            if (File.Exists(filename))
                File.Delete(filename);
            if (File.Exists(reversefilename))
                File.Delete(reversefilename);

            WriteToCSVFile(data.Candles.ToList(), filename);
            WriteToCSVFile(data.ReversedCandles.ToList(), reversefilename);
        }
        private static void WriteToCSVFile(List<Candle> candles, string filename)
        {
            File.WriteAllLines(filename, candles.Select(item =>
            {
                return String.Concat(
                    item.StandartTime.ToString("yyyyMMddHHmmss")
                    , ","
                    , String.Format(CultureInfo.InvariantCulture,"{0:F20}",item.open).TrimEnd('0')
                    , ","
                    , String.Format(CultureInfo.InvariantCulture, "{0:F20}", item.high).TrimEnd('0')
                    , ","
                    ,  String.Format(CultureInfo.InvariantCulture, "{0:F20}", item.low).TrimEnd('0')
                    , ","
                    , String.Format(CultureInfo.InvariantCulture, "{0:F20}", item.close).TrimEnd('0')
                    , ","
                    , String.Format(CultureInfo.InvariantCulture, "{0:F20}", item.volume).TrimEnd('0')
                    , ","
                    , (int)item.StandartTime.DayOfWeek
                    );
            }

            ).ToList());
        }
        #endregion

        #region AppendData
        /// <summary>
        /// complete the chart data file with the new one.
        /// use only for live mode
        /// fill candle and reverse candle
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filename"></param>
        public static void AppendChartDataFile(Chart data)
        {
            //Normal file
            AppendChartDataFiles(data.Candles.ToList(), FileHelper.getFullFileName(data.pair,false));
            //Reversed one
            AppendChartDataFiles(data.ReversedCandles.ToList(), FileHelper.getFullFileName(data.pair, true));
        }

        private static void AppendChartDataFiles(List<Candle> data, String FileName)
        {
            File.AppendAllLines(FileName, data.OrderBy(s => s.date).Select(item =>
            {
                return String.Concat(
                   item.StandartTime.ToString("yyyyMMddHHmmss")
                   , ","
                   , String.Format(CultureInfo.InvariantCulture, "{0:F20}", item.open).TrimEnd('0')
                   , ","
                   , String.Format(CultureInfo.InvariantCulture, "{0:F20}", item.high).TrimEnd('0')
                   , ","
                   , String.Format(CultureInfo.InvariantCulture, "{0:F20}", item.low).TrimEnd('0')
                   , ","
                   , String.Format(CultureInfo.InvariantCulture, "{0:F20}", item.close).TrimEnd('0')
                   , ","
                   , String.Format(CultureInfo.InvariantCulture, "{0:F20}", item.volume).TrimEnd('0')
                   , ","
                   , (int)item.StandartTime.DayOfWeek
                   );
            }

           ).ToList());
        }
        #endregion
    }
}
