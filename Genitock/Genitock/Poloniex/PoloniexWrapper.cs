using Genitock.Entity;
using Genitock.Entity.Poloniex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Genitock.Extension;
using Genitock.Interface;
using System.IO;
using Genitock.Entity.Poloniex.Market;
using Genitock.Entity.Poloniex.JSON;

namespace Genitock.Poloniex
{
    public class PoloniexWrapper : IBroker
    {
        readonly String ApiKey = ConfigurationManager.AppSettings["api"].ToString();
        readonly String secretKey = ConfigurationManager.AppSettings["secret"].ToString();
        /// <summary>
        /// if dry run is enable do not trade on market
        /// </summary>
        readonly Boolean DryRun = Convert.ToBoolean(ConfigurationManager.AppSettings["DryRun"]);
        /// <summary>
        /// 0 paire
        /// 1 tick debut
        /// 2 tick fin
        /// 4 periode
        /// </summary>
        readonly String GetUrl = "https://poloniex.com/public?command=";

        /// <summary>
        /// Post url to manage Poloniex account
        /// </summary>
        String PostUrl = "https://poloniex.com/tradingApi";


        #region HTTP GET
        public Chart GetChartData(Pair pair, DateTime dtStart, DateTime dtEnd, Period period)
        {
            String url = String.Format(GetUrl + "returnChartData&currencyPair={0}&start={1}&end={2}&period={3}"
                , pair
                , dtStart.getUnixTime()
                , dtEnd.getUnixTime()
                , (int)period);
            WebClient client = new WebClient();

            var content = client.DownloadString(url);
            Chart tickings = JsonConvert.DeserializeObject<Chart>("{	\"Candles\": " + content + "}");
            tickings.pair = pair;

            return tickings;

        }
        public Double EstimatedLastRate(Pair pair)
        {
            return returnMarketOrderBook(pair, 1).Bids.First().rate;
        }
        public MarketOrderBook returnMarketOrderBook(Pair pair, Int32 depth)
        {
            String url = String.Concat(GetUrl
                , "returnOrderBook&currencyPair="
                , pair.ToString()
                , "&depth="
                , depth);

            WebClient client = new WebClient();

            var content = client.DownloadString(url);
            RawMarketOrderBook rMarketBook = JsonConvert.DeserializeObject<RawMarketOrderBook>(content);

            return new MarketOrderBook(rMarketBook, pair);
        }

        #endregion

        #region HTTP POST
        /// <summary>
        /// return the amount available in a specific currency
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public Double ReturnBalance(Currencies currency)
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            client.Headers["key"] = ApiKey;

            String PostData = "command=returnBalances&nonce=" + DateTime.Now.getUnixTime();
            EncryptPost(client, PostData);
            String content = client.UploadString(PostUrl, "POST", PostData);

            //retrieve the right walled
            String targetWallet = content.Split(',').FirstOrDefault(p => p.Contains(currency.ToString()));
            return Convert.ToDouble(targetWallet.Split(':')[1].Replace('\"', '0'), CultureInfo.InvariantCulture);

        }



        private void EncryptPost(WebClient client, string PostData)
        {
            var keyByte = Encoding.UTF8.GetBytes(secretKey);
            using (var hmacsha512 = new HMACSHA512(keyByte))
            {
                hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(PostData));
                client.Headers["Sign"] = BitConverter.ToString(hmacsha512.Hash).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Sell the targeted currency to the original one
        /// </summary>
        /// <param name="pair">the target pair</param>
        /// <param name="rate">the price in the original currency</param>
        /// <param name="amount">the amount in the destination currency</param>
        /// <returns></returns>
        public TradeDone Sell(Pair pair, Double rate, Double amount)
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            client.Headers["key"] = ApiKey;

            String PostData = String.Concat("command=sell&nonce=", DateTime.Now.getUnixTime()
            , "&currencyPair=", pair.ToString()
            , "&rate=", String.Format(CultureInfo.InvariantCulture, "{0:F20}", rate).TrimEnd('0')
            , "&amount=", String.Format(CultureInfo.InvariantCulture, "{0:F20}", amount).TrimEnd('0')
            , "&immediateorcancel=1");

            EncryptPost(client, PostData);
            if (!DryRun)
            {
                String content = client.UploadString(PostUrl, "POST", PostData);
                return JsonConvert.DeserializeObject<TradeDone>(content);
            }
            else
            {
                TradeDone fake = new TradeDone();
                fake.orderNumber = "1";
                fake.resultingTrades = new List<ResultingTrade>();
                fake.resultingTrades.Add(new ResultingTrade { amount = amount, rate = rate, date = DateTime.Now, type = "sell" });
                return fake;
            }
        }

        /// <summary>
        /// Buy on market exchange
        /// </summary>
        /// <param name="pair">the target pair</param>
        /// <param name="rate">the price in the original currency</param>
        /// <param name="amount">the amount in the original currency</param>
        /// <returns></returns>
        public TradeDone Buy(Pair pair, Double rate, Double Initialamount)
        {
            //convert to the target currency because this is amount required in target currency for all exchange
            Double amount = Initialamount / rate;
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            client.Headers["key"] = ApiKey;

            String PostData = String.Concat("command=buy&nonce=", DateTime.Now.getUnixTime()
            , "&currencyPair=", pair.ToString()
            , "&rate=", String.Format(CultureInfo.InvariantCulture, "{0:F20}", rate).TrimEnd('0')
            , "&amount=", String.Format(CultureInfo.InvariantCulture, "{0:F20}", amount).TrimEnd('0')
             , "&immediateorcancel=1");

            EncryptPost(client, PostData);

            if (!DryRun)
            {
                String content = client.UploadString(PostUrl, "POST", PostData);
                return JsonConvert.DeserializeObject<TradeDone>(content); 
            }
            else
            {
                TradeDone fake = new TradeDone();
                fake.orderNumber = "1";
                fake.resultingTrades = new List<ResultingTrade>();
                fake.resultingTrades.Add(new ResultingTrade { amount = amount, rate = rate, date = DateTime.Now, type="buy" });
                return fake;
            }

        }

        public Boolean CancelOrder(String OrderNumber)
        {
			WebClient client = new WebClient();
			client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
			client.Headers["key"] = ApiKey;
            String Postdata = $"command=cancelOrder&nonce={DateTime.Now.getUnixTime()}&orderNumber={OrderNumber}";
			
            EncryptPost(client, Postdata);
			if (!DryRun)
			{
				String content = client.UploadString(PostUrl, "POST", Postdata);
                return content.Equals("{ \"success\":1}");
			}
			else
            {
                return true;
            }

        }
        #endregion
    }
}

