using Genitock.Entity.Poloniex;
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

 
        private static Pair _TradedPair;
        public TradingEnvironment(IBroker broker)
        {
            Boolean success;

            success = Enum.TryParse<Pair>(ConfigurationManager.AppSettings["Trading_Pair"], out _TradedPair);
            if (!success)
            {
                Console.WriteLine("Parameter Trading_Pair is invalid please see poloniex API for the correct format");
                Environment.Exit(0);
            }

     
            SourceWallet = new Wallet { currency = (Currencies)Enum.Parse(typeof(Currencies), _TradedPair.ToString().Split('_')[0]) };
            SourceWallet.amount = broker.ReturnBalance(SourceWallet.currency);

            TargetWallet = new Wallet { currency = (Currencies)Enum.Parse(typeof(Currencies), _TradedPair.ToString().Split('_')[0]) };
            TargetWallet.amount = broker.ReturnBalance(TargetWallet.currency);
        }

        public Boolean buy()
        {
            return false;
        }

        public Boolean Sell()
        {
            return false;
        }
    }
}
