using Genitock.Genotick;
using Genitock.Entity.Poloniex;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Utility
{
    public class FileHelper
    {
        /// <summary>
        /// //construct the path+ full name of a candle history data file
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="Reverse"></param>
        /// <returns></returns>
        public static String getFullFileName(Pair pair, Boolean Reverse)
        {            
            if (!Reverse)
                return Path.Combine(GenotickConfig.DataDirectory, pair.ToString() + ".csv");
            else
                return Path.Combine(GenotickConfig.DataDirectory, String.Concat("reverse_", pair.ToString(), ".csv"));

        }
    }
}
