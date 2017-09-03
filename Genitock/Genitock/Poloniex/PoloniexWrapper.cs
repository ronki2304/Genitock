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
        public Chart GetChartData(Pair pair,DateTime dtStart, DateTime dtEnd, Period period)
        { 
            //const String PostUrl = "https://poloniex.com/public?command=returnTicker";
            String url = String.Format(GetUrl
                , pair
                , dtStart.getUnixTime()
                ,dtEnd.getUnixTime()
                ,(int)period);
            WebClient client = new WebClient();

            var content = client.DownloadString(url);
            Chart tickings = JsonConvert.DeserializeObject<Chart>("{	\"MyArray\": " + content + "}");
            ////conversion to CSV
            //List<String> CSVline = new List<string>();

            //foreach (var item in tickings.MyArray.OrderBy(p => p.date))
            //{

            //    String Line = String.Concat(
            //        item.StandartTime.ToString("yyyyMMddHHmmss")
            //        , ","
            //        , item.open.ToString(CultureInfo.InvariantCulture)
            //        , ","
            //        , item.high.ToString(CultureInfo.InvariantCulture)
            //        , ","
            //        , item.low.ToString(CultureInfo.InvariantCulture)
            //        , ","
            //        , item.close.ToString(CultureInfo.InvariantCulture)
            //        );
            //    CSVline.Add(Line);
            //}
            return tickings;

        }
        private void ReturnBalance()
        {
            String PostUrl = "https://poloniex.com/tradingApi";
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

