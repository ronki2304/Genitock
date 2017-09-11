using Genitock.Entity.Poloniex;
using Genitock.Entity.Poloniex.Market;
using Genitock.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Trading
{
    /// <summary>
    /// this class represents all trading data,
    /// like wallet, current order....
    /// </summary>
    public class TradingEnvironment
    {
        /// <summary>
        /// return the wallet which will be used for trading
        /// </summary>
        public static Wallet SourceWallet { get; private set; }
        public static Wallet TargetWallet { get; private set; }
        /// <summary>
        /// return the target pair for trading
        /// </summary>
        public static Pair TradedPair { get { return _TradedPair; } }

        public IBroker _broker;
 
        private static Pair _TradedPair;
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

     
            SourceWallet = new Wallet { currency = (Currencies)Enum.Parse(typeof(Currencies), _TradedPair.ToString().Split('_')[0]) };
            SourceWallet.amount = _broker.ReturnBalance(SourceWallet.currency);

            TargetWallet = new Wallet { currency = (Currencies)Enum.Parse(typeof(Currencies), _TradedPair.ToString().Split('_')[1]) };
            TargetWallet.amount = _broker.ReturnBalance(TargetWallet.currency);
        }

        public Boolean Buy()
        {
            MarketOrderBook ob= _broker.returnMarketOrderBook(_TradedPair,20);
            Double amount = SourceWallet.amount;
            
            while (amount> Convert.ToDouble(ConfigurationManager.AppSettings["Minimum_trade"]))
            {
                TradeDone order = _broker.Buy(_TradedPair, ob.GetTheNextAsks().rate, amount);
                amount = amount - order.totalAmountDoneSourceCurrency;
            }
                return false;
        }

        public Boolean Sell()
        {
            MarketOrderBook ob = _broker.returnMarketOrderBook(_TradedPair, 20);
            Double amount = TargetWallet.amount;

            while (amount > Convert.ToDouble(ConfigurationManager.AppSettings["Minimum_trade"]))
            {
                TradeDone order = _broker.Sell(_TradedPair, ob.GetTheNextBids().rate, amount);
                amount = amount - order.totalAmountDoneTargetCurrency;
            }
            return false;
        }
    }
}
