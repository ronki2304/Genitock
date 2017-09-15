using Genitock.Entity;
using Genitock.Entity.Poloniex;
using Genitock.Entity.Poloniex.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Interface
{
    public interface IBroker
    {

        Double ReturnBalance(Currencies currency);

        Chart GetChartData(Pair pair, DateTime dtStart, DateTime dtEnd, Period period);

        MarketOrderBook returnMarketOrderBook(Pair pair, Int32 depth);

        TradeDone Sell(Pair pair, Double rate, Double amount);

        TradeDone Buy (Pair pair, Double rate, Double amount);

        Boolean CancelOrder(String OrderNumber);


    }
}
