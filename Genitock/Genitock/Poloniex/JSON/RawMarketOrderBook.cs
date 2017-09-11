using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Entity.Poloniex.JSON
{
    /// <summary>
    /// dedicated class for JSON parsing because it is very weird
    /// poloniex don't know how to format properly json
    /// but we forgive them because they gave us lot of data and hopefully money
    /// don't use it directly only for parsing json
    /// </summary>
    public class RawMarketOrderBook
    {
        [JsonProperty(PropertyName = "asks")]
        public IList<List<object>> Raw_asks;
        [JsonProperty(PropertyName = "bids")]
        public IList<List<object>> Raw_bids;
        public Int32 isFrozen;
        public Int32 seq;
    }
}
