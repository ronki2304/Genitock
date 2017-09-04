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
namespace Genitock.Poloniex
{
    public class PoloniexWrapper
    {
        readonly String ApiKey = ConfigurationManager.AppSettings["api"].ToString();
        readonly String secretKey = ConfigurationManager.AppSettings["secret"].ToString();
        /// <summary>
        /// 0 paire
        /// 1 tick debut
        /// 2 tick fin
        /// 4 periode
        /// </summary>
        const String GetUrl = "https://poloniex.com/public?command=returnChartData&currencyPair={0}&start={1}&end={2}&period={3}";

        /// <summary>
        /// Post url to manage Poloniex account
        /// </summary>
        String PostUrl = "https://poloniex.com/tradingApi";
        public Chart GetChartData(Pair pair,DateTime dtStart, DateTime dtEnd, Period period)
        { 
            String url = String.Format(GetUrl
                , pair
                , dtStart.getUnixTime()
                ,dtEnd.getUnixTime()
                ,(int)period);
            WebClient client = new WebClient();

            var content = client.DownloadString(url);
            Chart tickings = JsonConvert.DeserializeObject<Chart>("{	\"Candles\": " + content + "}");
            tickings.pair = pair;
            
            return tickings;

        }
        private void ReturnBalance()
        {
            
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            client.Headers["key"] = ApiKey;

            String PostData = "command=returnBalances&nonce=" + DateTime.Now.getUnixTime();

            var keyByte = Encoding.UTF8.GetBytes(secretKey);
            using (var hmacsha512 = new HMACSHA512(keyByte))
            {
                hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(PostData));
                client.Headers["Sign"] = BitConverter.ToString(hmacsha512.Hash).Replace("-", "").ToLower();
                var content = client.UploadString(PostUrl, "POST", PostData);
                Console.WriteLine(content);
            }
        }
        int GetNonce()
        {
            return (int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
        }

      

    }
}

