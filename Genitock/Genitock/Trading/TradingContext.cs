using System;
using Genitock.Entity.Poloniex;
using Genitock.Entity.Poloniex.Market;

namespace Genitock.Trading
{
    /// <summary>
    /// this class stores all data from the trdin like the profit, the last position, the higher profit....
    /// all those data are stored in an xml file
    /// this class is only used by tradingenvironement
    /// </summary>

   public class TradingContext
    {
        public TradingStatus status { get; set; }
        public Operation CurrentOperation { get; set; }
        public DateTime? Position { get; set; }
        public Double Profit { get; set; }
        public Double HighestProfit { get; set; }
		/// <summary>
		/// store how much is traded
		/// </summary>
		/// <value>The amount traded.</value>
		public Double? AmountTraded { get; set; }
        public DateTime? MaxDrawDown { get; set; }
         
    }
}
