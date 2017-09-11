using Genitock.Entity.Poloniex.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Entity.Poloniex.Market
{
    public class MarketOrderBook
    {
        public MarketOrderBook(RawMarketOrderBook rbook, Pair ppair)
        {
            Bids = new List<MarketOrder>();
            Asks = new List<MarketOrder>();
            foreach (var item in rbook.Raw_bids.OrderByDescending(p => Convert.ToDouble(p[0], CultureInfo.InvariantCulture)))
            {
                Bids.Add(new MarketOrder { Amount = Convert.ToDouble(item[1]), rate = Convert.ToDouble(item[0],CultureInfo.InvariantCulture) });
            }
            foreach (var item in rbook.Raw_asks.OrderBy(p => Convert.ToDouble(p[0], CultureInfo.InvariantCulture)))
            {
                Asks.Add(new MarketOrder { Amount = Convert.ToDouble(item[1]), rate = Convert.ToDouble(item[0],CultureInfo.InvariantCulture) });
            }

            pair = ppair;
        }
        public List<MarketOrder> Bids { get; private set; }
        public List<MarketOrder> Asks { get; private set; }

        private Int32 BidsIndex = 0;
        private Int32 AsksIndex = 0;

        public MarketOrder GetTheNextAsks()
        {
            if (AsksIndex < Asks.Count - 1)
            {
                AsksIndex++;
                return Asks[AsksIndex];
            }
            return null;
        }

        public MarketOrder GetTheNextBids()
        {
            if (BidsIndex < Bids.Count - 1)
            {
                BidsIndex++;
                return Bids[BidsIndex];
            }
            return null;
        }
        public Pair pair { get; private set; }
    }
}
