using System;
namespace Genitock.Entity.Poloniex
{
    public class PoloniexArg
    {
		public String Pair { get; set; }
		public Double Rate { get; set; }
		public PoloniexArg(String pair, Double rate)
		{
			Pair = pair;
			Rate = rate;
		}
    }
}
