using System;
namespace Genitock.Entity.Poloniex
{
    public class TickerArg
    {
            public String Pair { get; set; }
            public Decimal Rate { get; set; }

            public TickerArg(String pair, Decimal rate)
        {
            Pair = pair;
            Rate = rate;
        }

    }
}
