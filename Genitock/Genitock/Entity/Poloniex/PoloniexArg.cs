using System;
namespace Genitock.Entity.Poloniex
{
    public class PoloniexArg
    {
		public String Pair { get; set; }
		public Decimal Rate { get; set; }
		public PoloniexArg(String pair, Decimal rate)
		{
			Pair = pair;
			Rate = rate;
		}
    }
}
