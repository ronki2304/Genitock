using Genitock.Entity.Poloniex;
using Genitock.Entity.Poloniex.Market;
using Genitock.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genitock.Poloniex.Live;
using Genitock.Entity.Live;
using System.Xml.Serialization;
using System.IO;

namespace Genitock.Trading
{
    /// <summary>
    /// this class represents all trading data,
    /// like wallet, current order....
    /// </summary>
    public class TradingEnvironment
    {

        private TradingContext context; 
      

        /// <summary>
        /// return the wallet which will be used for trading
        /// </summary>
        public Wallet SourceWallet { get; private set; }
        public Wallet TargetWallet { get; private set; }
        public TradingStatus state { get; private set; }
        /// <summary>
        /// return the target pair for trading
        /// </summary>
        public Pair TradedPair { get { return _TradedPair; } }

        public IBroker _broker;

        public ITicker _ticker;
 
        private Pair _TradedPair;

        /// <summary>
        /// store the stop limit rate from buy action
        /// </summary>
        private static Double StopLimitBids;

        public TradingEnvironment(IBroker broker, ITicker ticker)
        {
            
            Boolean success;
            _broker = broker;
            _ticker = ticker;

            success = Enum.TryParse<Pair>(ConfigurationManager.AppSettings["Trading_Pair"], out _TradedPair);
            if (!success)
            {
                Console.WriteLine("Parameter Trading_Pair is invalid please see poloniex API for the correct format");
                Environment.Exit(0);
            }

            RefreshWallet();
            state = SourceWallet.amount > TargetWallet.amount ? TradingStatus.OutMarket:TradingStatus.InMarket;
            context = new TradingContext();
        }

        private void RefreshWallet()
        {
            SourceWallet = new Wallet { currency = (Currencies)Enum.Parse(typeof(Currencies), _TradedPair.ToString().Split('_')[0]) };
            SourceWallet.amount = _broker.ReturnBalance(SourceWallet.currency);

            TargetWallet = new Wallet { currency = (Currencies)Enum.Parse(typeof(Currencies), _TradedPair.ToString().Split('_')[1]) };
            TargetWallet.amount = _broker.ReturnBalance(TargetWallet.currency);
        }

        public void Buy()
        {
            Console.WriteLine("Init buy process");
            MarketOrderBook ob= _broker.returnMarketOrderBook(_TradedPair,20);
            Double amount = SourceWallet.amount;

            TradeDone allorders = new TradeDone();
            allorders.resultingTrades = new List<ResultingTrade>();

            while (amount> Convert.ToDouble(ConfigurationManager.AppSettings["Minimum_trade"]))
            {
                TradeDone order = _broker.Buy(_TradedPair, ob.GetTheNextAsks().rate, amount);
                if (order.resultingTrades.Count()==0)
                {
                    Boolean cleanSituation = false;
                    Console.WriteLine("order not executed. clean it");
                    //need to cancel order to rate to high
                    while (!cleanSituation)
                        cleanSituation=_broker.CancelOrder(order.orderNumber);
                    //restart the process
                    Console.WriteLine("clean complete continue buy process");
                    continue;
                }
                amount = amount - order.totalAmountDoneSourceCurrency;
                allorders.resultingTrades.AddRange(order.resultingTrades);
            }

            //compute the average rate to determine the stop limit
            StopLimitBids = allorders.AverageRate * Convert.ToDouble(ConfigurationManager.AppSettings["StopLoss"]);
            Console.WriteLine($"buy done stop limit rate {StopLimitBids}");
            _ticker.onTick+= WatchStopLimit;
            state = TradingStatus.InMarket;
            RefreshWallet();
                
        }

        public void Sell()
        {
            Console.WriteLine("Init sell process");
            MarketOrderBook ob = _broker.returnMarketOrderBook(_TradedPair, 20);
            Double amount = TargetWallet.amount;

            while (amount > Convert.ToDouble(ConfigurationManager.AppSettings["Minimum_trade"]))
            {
                TradeDone order = _broker.Sell(_TradedPair, ob.GetTheNextBids().rate, amount);
				if (order.resultingTrades.Count() == 0)
				{
					Boolean cleanSituation = false;
					Console.WriteLine("order not executed. clean it");
					//need to cancel order to rate to high
					while (!cleanSituation)
						cleanSituation = _broker.CancelOrder(order.orderNumber);
					//restart the process
					Console.WriteLine("clean complete continue sell process");
					continue;
				}

                amount = amount - order.totalAmountDoneTargetCurrency;
            }
            Console.WriteLine("disable ticker action");
            _ticker.onTick -= WatchStopLimit;
			RefreshWallet();
            state = TradingStatus.OutMarket;
        }

        public Chart GetChartData(Pair pair, DateTime dtStart, DateTime dtEnd, Period period)
        {
            return _broker.GetChartData(pair,dtStart,dtEnd,period);
        }

        /// <summary>
        /// check if the stop limit is raised
        /// </summary>
        /// <param name="source">Source.</param>
        /// <param name="e">E.</param>
		void WatchStopLimit(object source, TickerArgument e)
		{
			Console.WriteLine($"Date {DateTime.Now} Pair {e.Pair} Rate {e.Rate}");
			Console.WriteLine($"Stop loss rate : {StopLimitBids}");
            if (e.HighestBid < StopLimitBids)
				Sell();
		}

        void SaveTradingContext()
        {
			XmlSerializer xs = new XmlSerializer(typeof(TradingContext));
			TextWriter WriteFileStream = new StreamWriter(@"test.xml");
            xs.Serialize(WriteFileStream, context);
			return;
        }
    }
}
