using System;
using Genitock.Delegate;
using System.Threading.Tasks;
using Genitock.Entity.Poloniex;

using System.Configuration;
using WampSharp.V2;
using WampSharp.V2.Fluent;
using WampSharp.V2.Client;

namespace Genitock.Poloniex.Live
{
    public  static class Ticker
    {
        public static event OnTick onTick;
		static WampChannelReconnector reconnector;

        static Ticker()
        {
	

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
			                 if (true || currencyPair == ConfigurationManager.AppSettings["Trading_Pair"])
			      {
			          Console.WriteLine($"Currencypair: {currencyPair}, Last: {last}, Date: {DateTime.Now}");
			          onTick(null, new PoloniexArg(currencyPair, last));

			      }
			  },
			  ex => {
			      Console.WriteLine($"Oh no! {ex}");
			  });

			};


			reconnector =new WampChannelReconnector(channel, _connect);

			reconnector.Start();
        }
    }
}
