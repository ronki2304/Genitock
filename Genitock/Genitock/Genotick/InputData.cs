using Genitock.Entity;
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
        public static void SaveToCSV(Pair pair,Chart data, String outputDirectory)  
        {
            String filename = Path.Combine(outputDirectory, String.Concat(pair.ToString(),".csv"));
            String reversefilename = Path.Combine(outputDirectory, String.Concat("reverse_",pair.ToString(), ".csv"));

            if (File.Exists(filename))
                File.Delete(filename);
            if (File.Exists(reversefilename))
                File.Delete(reversefilename);


            //compute day of week
            

            File.WriteAllLines(filename, data.MyArray.Select(item => {
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
                    ,","
                    ,item.volume.ToString(CultureInfo.InvariantCulture)
                    ,","
                    ,(int) item.StandartTime.DayOfWeek
                    );
            }

            ).ToList());

        }

        void ReverseData()
        {

        }
    }
}
