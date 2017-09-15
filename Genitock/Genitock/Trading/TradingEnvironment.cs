﻿using Genitock.Entity.Poloniex;
using Genitock.Entity.Poloniex.Market;
using Genitock.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genitock.Poloniex.Live;

namespace Genitock.Trading
{
    /// <summary>
    /// this class represents all trading data,
    /// like wallet, current order....
    /// </summary>
    public class TradingEnvironment
    {
        void WatchStopLimit(object source, Entity.Poloniex.PoloniexArg e)
        {
            Console.WriteLine($"Date {DateTime.Now} Pair {e.Pair} Rate {e.Rate}");
        }

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
 
        private Pair _TradedPair;

        /// <summary>
        /// store the stop limit rate from buy action
        /// </summary>
        private static Double StopLimitrate;

        private TradingData tradersetup;

        public TradingEnvironment(IBroker broker)
        {
            Boolean success;
            _broker = broker;

            success = Enum.TryParse<Pair>(ConfigurationManager.AppSettings["Trading_Pair"], out _TradedPair);
            if (!success)
            {
                Console.WriteLine("Parameter Trading_Pair is invalid please see poloniex API for the correct format");
                Environment.Exit(0);
            }

            RefreshWallet();
            state = SourceWallet.amount > TargetWallet.amount ? TradingStatus.OutMarket:TradingStatus.InMarket;
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
            StopLimitrate = allorders.AverageRate * Convert.ToDouble(ConfigurationManager.AppSettings["StopLoss"]);
            Console.WriteLine($"buy done average rate {StopLimitrate}");
            Ticker.onTick+= WatchStopLimit;
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
            Ticker.onTick -= WatchStopLimit;
			RefreshWallet();
            state = TradingStatus.OutMarket;
        }

        public Chart GetChartData(Pair pair, DateTime dtStart, DateTime dtEnd, Period period)
        {
            return _broker.GetChartData(pair,dtStart,dtEnd,period);
        }
    }
}
