using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Entity
{ 
   public  class Chart
    { 
        public IList<Candle> Candles { get; set; }
        public IList<Candle> ReversedCandles
        {
            get
            {
                return Candles.Select(p => new Candle
                {
                    date = p.date
                    ,
                    close = 1 / p.close
                    ,
                    high = 1 / p.high
                    ,
                    low = 1 / p.low
                    ,
                    open = 1 / p.open
                    ,
                    quotevolume = p.quotevolume
                    ,
                    volume = p.volume
                    ,
                    weightedAverage = p.weightedAverage
                }).ToList();
            }
        }
    }

    public class Candle
    {
        public DateTime StandartTime
        {
            get
            {
                
                // First make a System.DateTime equivalent to the UNIX Epoch.
                DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

                // Add the number of seconds in UNIX timestamp to be converted.
                dateTime = dateTime.AddSeconds(date);
                return dateTime;
            }
        }
        public Int64 date { get; set; }
        public Double high { get; set; }
        public Double low { get; set; }
        public Double open { get; set; }
        public Double close { get; set; }
        public Double volume { get; set; }
        public Double quotevolume { get; set; }
        public Double weightedAverage { get; set; }
    }

}
