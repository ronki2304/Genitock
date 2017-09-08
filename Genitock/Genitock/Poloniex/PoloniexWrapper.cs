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

namespace Genitock.Poloniex
{
    public class PoloniexWrapper : IBroker
    {
        readonly String ApiKey = ConfigurationManager.AppSettings["api"].ToString();
        readonly String secretKey = ConfigurationManager.AppSettings["secret"].ToString();
        /// <summary>
        /// 0 paire
        /// 1 tick debut
        /// 2 tick fin
        /// 4 periode
        /// </summary>
        readonly String GetUrl = "https://poloniex.com/public?command=returnChartData&currencyPair={0}&start={1}&end={2}&period={3}";

        /// <summary>
        /// Post url to manage Poloniex account
        /// </summary>
        String PostUrl = "https://poloniex.com/tradingApi";
        public Chart GetChartData(Pair pair, DateTime dtStart, DateTime dtEnd, Period period)
        {
            String url = String.Format(GetUrl
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

        public Int32 Sell(Pair pair, Double rate, Double amount)
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            client.Headers["key"] = ApiKey;

            String PostData = String.Concat("command=sell&nonce=", DateTime.Now.getUnixTime()
            , "&currencyPair=", pair.ToString()
            , "&rate=", String.Format(CultureInfo.InvariantCulture, "{0:F20}", rate).TrimEnd('0')
            , "&amount=", String.Format(CultureInfo.InvariantCulture, "{0:F20}", amount).TrimEnd('0') );

            EncryptPost(client, PostData);

            String content = client.UploadString(PostUrl, "POST", PostData);
            Console.WriteLine(content);
            File.WriteAllText("titi.txt", content);
            return 0;

        }

    }
}

