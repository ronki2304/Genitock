using System;
using Genitock.Delegate;
using System.Threading.Tasks;
using Genitock.Entity.Poloniex;

using System.Configuration;
using WampSharp.V2;
using WampSharp.V2.Fluent;
using WampSharp.V2.Client;
using System.Threading;

namespace Genitock.Poloniex.Live
{
    public static class Ticker
    {
        public static event OnTick onTick;
        static WampChannelReconnector reconnector;
        private static DateTime LastTickReceived;
        private static Timer tictac;
        private static readonly Pair pair = (Pair)Enum.Parse(typeof(Pair), ConfigurationManager.AppSettings["Trading_Pair"]);

        static Ticker()
        {
            tictac = new Timer(HandleTimerCallback,null,0,30000);
           LastTickReceived = DateTime.Now;
            IWampChannelFactory factory = new WampChannelFactory();
            IWampChannel channel = factory.ConnectToRealm("realm1")
                                          .WebSocketTransport("wss://api.poloniex.com")
                                          .JsonSerialization()
                                          .Build();
            Func<Task> _connect = async () =>
            {
                await channel.Open();
                var tickerSubject = channel.RealmProxy.Services.GetSubject("ticker");

                IDisposable subscription = tickerSubject.Subscribe(evt =>
                {
                    var currencyPair = evt.Arguments[0].Deserialize<string>();
                    var last = evt.Arguments[1].Deserialize<decimal>();
                    if (currencyPair == pair.ToString())
                    {
                        LastTickReceived = DateTime.Now;
                        Console.WriteLine($"Currencypair: {currencyPair}, Last: {last}, Date: {DateTime.Now}");
                        onTick(null, new PoloniexArg(currencyPair, last));

                    }
                },
                ex =>
                {
                    Console.WriteLine($"Oh no! {ex}");
                });

            };


            reconnector = new WampChannelReconnector(channel, _connect);

            reconnector.Start();
        }

        /// <summary>
        /// if wamp don't return data each 30second call the get data
        /// </summary>
        /// <param name="state">State.</param>
        static void HandleTimerCallback(object state)
        {
            
            TimeSpan ts = DateTime.Now.Subtract(LastTickReceived);
            if (ts.TotalSeconds > 30)
            {
                PoloniexWrapper pw = new PoloniexWrapper();

                onTick(null, new PoloniexArg("manual", Convert.ToDecimal(pw.EstimatedLastRate(pair))));
            }
        }

       
    }
}
