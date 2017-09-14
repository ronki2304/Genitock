using System;
using Genitock.Delegate;
using System.Threading.Tasks;
using Genitock.Entity.Poloniex;
using WampSharp.V2;

namespace Genitock.Poloniex.Live
{
    public  static class Ticker
    {
        public static event OnTick onTick;
		static WampChannelReconnector reconnector;

        static Ticker()
        {
			var channelFactory = new DefaultWampChannelFactory();
			var channel = channelFactory.CreateJsonChannel("wss://api.poloniex.com", "realm1");
			Func<Task> _connect = async () =>
			{
				await channel.Open();
				var tickerSubject = channel.RealmProxy.Services.GetSubject("ticker");

				IDisposable subscription = tickerSubject.Subscribe(evt =>
				{
					var currencyPair = evt.Arguments[0].Deserialize<string>();
					var last = evt.Arguments[1].Deserialize<decimal>();
					if (currencyPair == "BTC_ETH")
					{
						//Console.WriteLine($"Currencypair: {currencyPair}, Last: {last}, Date: {DateTime.Now}");
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
