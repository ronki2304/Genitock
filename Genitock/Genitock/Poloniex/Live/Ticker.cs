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
    /// <summary>
    /// retrieve real time ticker via wamp
    /// </summary>
    public static class Ticker
    {
        /// <summary>
        /// raise event on each tick
        /// </summary>
        public static event OnTick onTick;
        static WampChannelReconnector reconnector;

        /// <summary>
        /// last tick received by WAMP
        /// </summary>
        private static DateTime LastTickReceived;

        /// <summary>
        /// wamp may be overloaded so switch to http get when it ook too long time
        /// </summary>
        private static Timer tictac;

        /// <summary>
        /// traded pair
        /// </summary>
        private static readonly Pair pair = (Pair)Enum.Parse(typeof(Pair), ConfigurationManager.AppSettings["Trading_Pair"]);

        static Ticker()
        {
            LastTickReceived = DateTime.Now;
            //raise every 30sec
            tictac = new Timer(HandleTimerCallback,null,0,30000);
           
            //WAMP init
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
                    var last = evt.Arguments[1].Deserialize<Double>();
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

                onTick(null, new PoloniexArg("manual", pw.EstimatedLastRate(pair)));
            }
        }

       
    }
}
