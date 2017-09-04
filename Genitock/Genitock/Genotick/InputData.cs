using Genitock.Entity;
using Genitock.Entity.Genotick;
using Genitock.Entity.Poloniex;
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
        public static void WriteToCSVFile(Pair pair, Chart data, String outputDirectory)
        {
            String filename = Path.Combine(outputDirectory, String.Concat(pair.ToString(), ".csv"));
            String reversefilename = Path.Combine(outputDirectory, String.Concat("reverse_", pair.ToString(), ".csv"));

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
                    , item.open.ToString(CultureInfo.InvariantCulture)
                    , ","
                    , item.high.ToString(CultureInfo.InvariantCulture)
                    , ","
                    , item.low.ToString(CultureInfo.InvariantCulture)
                    , ","
                    , item.close.ToString(CultureInfo.InvariantCulture)
                    , ","
                    , item.volume.ToString(CultureInfo.InvariantCulture)
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
        /// usefull for live mode before runnning genotick
        /// take care of reversed data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filename"></param>
        public static void AppendChartDataFile(Chart data)
        {
            //Normal file
            AppendChartDataFiles(data.Candles.ToList(), GenotickConfig.CurrenciesDataFile.First());
            //Reversed one
            AppendChartDataFiles(data.ReversedCandles.ToList(), GenotickConfig.ReverseCurrenciesDataFile.First());
        }

        private static void AppendChartDataFiles(List<Candle> data, String FileName)
        {
            File.AppendAllLines(FileName, data.OrderBy(s => s.date).Select(item =>
            {
                return String.Concat(
                    item.StandartTime.ToString("yyyyMMddHHmmss")
                    , ","
                    , item.open.ToString(CultureInfo.InvariantCulture)
                    , ","
                    , item.high.ToString(CultureInfo.InvariantCulture)
                    , ","
                    , item.low.ToString(CultureInfo.InvariantCulture)
                    , ","
                    , item.close.ToString(CultureInfo.InvariantCulture)
                    , ","
                    , item.volume.ToString(CultureInfo.InvariantCulture)
                    , ","
                    , (int)item.StandartTime.DayOfWeek
                    );
            }

           ).ToList());
        }
        #endregion
    }
}
