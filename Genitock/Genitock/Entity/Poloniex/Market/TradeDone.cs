using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Entity.Poloniex.Market
{
    public class ResultingTrade
    {
        public Double amount { get; set; }
        public DateTime date { get; set; }
        public Double rate { get; set; }
        public Double total { get; set; }
        public string tradeID { get; set; }
        public string type { get; set; }
    }

    public class TradeDone
    {
        public string orderNumber { get; set; }
        public List<ResultingTrade> resultingTrades { get; set; }

        public Double totalAmountDoneTargetCurrency
        {
            get
            {
                if (resultingTrades == null)
                    return 0;
                return resultingTrades.Sum(p => p.amount);
            }
        }
        public Double totalAmountDoneSourceCurrency
        {
            get
            {
                if (resultingTrades == null)
                    return 0;
                return resultingTrades.Sum(p => p.amount*p.rate);
            }
        }
    }
}
