using System;
namespace Genitock.Entity.Live
{

    /// <summary>
    /// this class represent the tick event raised from ticker
    /// </summary>
    public class TickerArgument
    {
		public String Pair { get; set; }
		public Double Rate { get; set; }
        public Double LowestAsk { get; set; }
        public Double HighestBid { get; set; }
		
    }
}
